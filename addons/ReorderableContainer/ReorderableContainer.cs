using System.Collections.Generic;
using Godot;
using Godot.Collections;

[Tool]
public partial class ReorderableContainer : Container
{
	[Signal]
	public delegate void ReorderedEventHandler(int from, int to);

	private const float dropZoneExtend = 2000;

	[Export]
	private float holdDuration = 0.5f;

	[Export(PropertyHint.Range, "3,30,0.01,or_greater,or_less")]
	private float speed = 10f;

	[Export]
	private float _separation;

	[Export]
	private float separation
	{
		get => _separation;
		set
		{
			if (value == _separation || value < 0)
				return;
			_separation = value;
			OnSortChildren();
		}
	}

	//horrible hack to hide things in editor - can't modify PropertyUsageFlags otherwise :/
	public override void _ValidateProperty(Dictionary property)
	{
		base._ValidateProperty(property);
		var propertyName = property["name"].AsStringName();
		if (propertyName == "_separation")
		{
			property["usage"] = (int)PropertyUsageFlags.NoEditor;
		}
		else if (propertyName == "separation")
		{
			property["usage"] = (int)PropertyUsageFlags.Editor;
		}
	}

	[Export]
	private ScrollContainer scrollContainer;

	// The maximum speed of auto scroll.
	[Export]
	private float autoScrollSpeed = 10f;

	// The percentage of how much space auto scroll will take in [ScrollContainer][br][br]
	// [b]Example:[/b] If [code]auto_scroll_range[/code] is 30% (0.3) && [ScrollContainer] height is 100 px, 
	// upper part will be 0 to 30 px && lower part will be 70 to 100 px.
	[Export(PropertyHint.Range, "0, 0.5")]
	private float autoScrollRange = 0.3f;

	// The scrolling threshold in pixel. In a nutshell, user will have hard time trying to drag a child if it too low
	// && user will accidentally drag a child when scrolling if it too high.
	[Export]
	private float scrollThreshold = 30f;

	// Uses when debugging
	[Export]
	private bool isDebugging = false;

	private float scrollStartingPoint = 0;
	private bool isSmoothScroll = false;

	private List<Rect2> dropZones = new();
	private int dropZoneIndex = -1;
	private List<Rect2> expectChildRect = new();

	private Control focusChild;
	private bool isPress = false;
	private bool isHold = false;
	private double currentDuration = 0f;
	private bool isUsingProcess = false;

	public override void _Ready()
	{
		base._Ready();
		if (scrollContainer == null && this.GetParent() is ScrollContainer scrollParent)
			scrollContainer = scrollParent;

		if (scrollContainer != null && scrollContainer.HasMethod("handle_overdrag"))
			isSmoothScroll = true;

		ProcessMode = ProcessModeEnum.Pausable;
		AdjustExpectedChildRect();
		
		//SortChildren -= OnSortChildren;
		SortChildren += OnSortChildren;

		//GetTree().NodeAdded -= OnNodeAdded;
		GetTree().NodeAdded += OnNodeAdded;
		
		OnSortChildren();
	}

	public override void _GuiInput(InputEvent @event)
	{
		base._GuiInput(@event);
		if (@event is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.ButtonIndex == MouseButton.Left)
		{
			foreach (var child in GetChildren())
			{
				if (child is not Control childControl)
					continue;

				if (childControl.GetRect().HasPoint(GetLocalMousePosition()) && mouseButtonEvent.IsPressed())
				{
					focusChild = childControl;
					isPress = true;
				}
				else if (!mouseButtonEvent.IsPressed())
				{
					isPress = false;
					isHold = false;
				}
			}
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Engine.IsEditorHint())
			return;

		HandleInput(delta);

		if (currentDuration >= holdDuration != isHold)
		{
			isHold = currentDuration >= holdDuration;
			if (isHold)
				OnStartDragging();
		}

		if (isHold)
		{
			_handle_dragging_child_pos(delta);
			if (scrollContainer != null)
				HandleAutoScroll(delta);
		}
		else if (!isHold && dropZoneIndex != -1)
			OnStopDragging();

		if (isUsingProcess)
			OnSortChildren(delta);
	}


	private void HandleInput(double delta)
	{
		if (scrollContainer != null && isPress && !isHold)
		{
			var scrollPoint = CustomMinimumSize.X == 0 ? scrollContainer.ScrollVertical : scrollContainer.ScrollHorizontal;
			if (currentDuration == 0)
			{
				scrollStartingPoint = scrollPoint;
			}
			else
			{
				// If user scroll more than scroll_threshold, press is abort.
				isPress = Mathf.Abs(scrollPoint - scrollStartingPoint) <= scrollThreshold;
			}
		}
		currentDuration = isPress ? currentDuration + delta : 0;
	}


	private void OnStartDragging()
	{
		// Force _on_sort_children to use process update for linear interpolation
		isUsingProcess = true;
		focusChild.ZIndex = 1;
		// Workaround for SmoothScroll addon
		if (isSmoothScroll)
			scrollContainer.ProcessMode = ProcessModeEnum.Disabled;
		foreach (var child in GetVisibleChildren())
			child.PropagateCall("set_mouse_filter", [MouseFilter == MouseFilterEnum.Ignore]);
	}

	private void OnStopDragging()
	{
		focusChild.ZIndex = 0;
		var focusChildIndex = focusChild.GetIndex();
		MoveChild(focusChild, dropZoneIndex);
		EmitSignal(SignalName.Reordered, focusChildIndex, dropZoneIndex);
		focusChild = null;
		dropZoneIndex = -1;
		if (isSmoothScroll)
		{
			scrollContainer.Position = new Vector2(-scrollContainer.ScrollHorizontal, -scrollContainer.ScrollVertical);
			scrollContainer.ProcessMode = ProcessModeEnum.Inherit;
		}
		foreach (var child in GetVisibleChildren())
			child.PropagateCall("set_mouse_filter", [MouseFilter == MouseFilterEnum.Pass]);
	}

	private void OnNodeAdded(Node node)
	{
		if (node is Control control && !Engine.IsEditorHint())
			control.MouseFilter = MouseFilterEnum.Pass;
	}

	private void _handle_dragging_child_pos(double delta)
	{
		if (CustomMinimumSize.X == 0)
		{
			var targetPos = GetLocalMousePosition().Y - (focusChild.Size.Y / 2.0);
			focusChild.Position = new Vector2(focusChild.Position.X, (float)Mathf.Lerp(focusChild.Position.Y, targetPos, (float)delta * speed));
		}
		else
		{
			var targetPos = GetLocalMousePosition().X - (focusChild.Size.X / 2.0);
			focusChild.Position = new Vector2((float)Mathf.Lerp(focusChild.Position.X, targetPos, delta * speed), focusChild.Position.Y);
		}

		// Update drop zone index
		Vector2 childCenterPos = focusChild.GetRect().GetCenter();
		for (int i = 0; i < dropZones.Count; i++)
		{
			var dropZone = dropZones[i];
			if (dropZone.HasPoint(childCenterPos))
			{
				dropZoneIndex = i;
				break;
			}
			else if (i == dropZones.Count - 1)
				dropZoneIndex = -1;
		}
	}


	private void HandleAutoScroll(double delta)
	{
		var mouseGPos = GetGlobalMousePosition();
		var scrollGRect = scrollContainer.GetGlobalRect();
		if (CustomMinimumSize.X == 0)
		{
			var upper = scrollGRect.Position.Y + (scrollGRect.Size.Y * autoScrollRange);
			var lower = scrollGRect.Position.Y + (scrollGRect.Size.Y * (1.0 - autoScrollRange));

			if (upper > mouseGPos.Y)
			{
				var factor = (upper - mouseGPos.Y) / (upper - scrollGRect.Position.Y);
				scrollContainer.ScrollVertical -= (int)(delta * (float)(autoScrollSpeed) * 150.0 * factor);
			}
			else if (lower < mouseGPos.Y)
			{
				var factor = (mouseGPos.Y - lower) / (scrollGRect.End.Y - lower);
				scrollContainer.ScrollVertical += (int)(delta * (float)(autoScrollSpeed) * 150.0 * factor);
			}
			else
			{
				scrollContainer.ScrollVertical = scrollContainer.ScrollVertical;
			}
		}
		else
		{
			var left = scrollGRect.Position.X + (scrollGRect.Size.X * autoScrollRange);
			var right = scrollGRect.Position.X + (scrollGRect.Size.X * (1.0 - autoScrollRange));

			if (left > mouseGPos.X)
			{
				var factor = (left - mouseGPos.X) / (left - scrollGRect.Position.X);
				scrollContainer.ScrollHorizontal -= (int)(delta * (float)(autoScrollSpeed) * 150.0 * factor);
			}
			else if (right < mouseGPos.X)
			{
				var factor = (mouseGPos.X - right) / (scrollGRect.End.X - right);
				scrollContainer.ScrollHorizontal += (int)(delta * (float)(autoScrollSpeed) * 150.0 * factor);
			}
			else
				scrollContainer.ScrollHorizontal = scrollContainer.ScrollHorizontal;
		}
	}

	private void OnSortChildren()
	{
		OnSortChildren(-1.0);
	}

	private void OnSortChildren(double delta)
	{
		if (isUsingProcess && delta == -1.0)
			return;
		
		AdjustExpectedChildRect();
		AdjustChildRect(delta);
		AdjustDropZoneRect();
	}

	private void AdjustExpectedChildRect()
	{
		expectChildRect.Clear();
		var children = GetVisibleChildren();
		float endPoint = 0f;
		for (int i = 0; i < children.Length; i++)
		{
			var child = children[i];
			var minSize = child.GetCombinedMinimumSize();
			if (CustomMinimumSize.X == 0)
			{
				if (i == dropZoneIndex)
					endPoint += focusChild.Size.Y + separation;

				expectChildRect.Add(new Rect2(new Vector2(0, endPoint), new Vector2(Size.X, minSize.Y)));
				endPoint += minSize.Y + separation;
			}
			else
			{
				if (i == dropZoneIndex)
					endPoint += focusChild.Size.X + separation;

				expectChildRect.Add(new Rect2(new Vector2(endPoint, 0), new Vector2(minSize.X, Size.Y)));
				endPoint += minSize.X + separation;
			}
		}
	}


	private void AdjustChildRect(double delta = -1.0)
	{
		var children = GetVisibleChildren();
		if (children.Length == 0)
			return;

		var isAnimating = false;
		var endPoint = 0.0;
		for (int i = 0; i < children.Length; i++)
		{
			var child = children[i];
			if (child.Position == expectChildRect[i].Position && child.Size == expectChildRect[i].Size)
				continue;

			if (isUsingProcess)
			{
				isAnimating = true;
				child.Position = new Vector2((float)Mathf.Lerp(child.Position.X, expectChildRect[i].Position.X, delta * speed),
					(float)Mathf.Lerp(child.Position.Y, expectChildRect[i].Position.Y, delta * speed));
				child.Size = expectChildRect[i].Size;
				if ((child.Position - expectChildRect[i].Position).Length() <= 1.0)
					child.Position = expectChildRect[i].Position;
			}
			else
			{
				child.Position = expectChildRect[i].Position;
				child.Size = expectChildRect[i].Size;
			}
		}

		var lastChild = children[^1];
		if (CustomMinimumSize.X == 0)
		{
			if (isUsingProcess && dropZoneIndex == children.Length)
				CustomMinimumSize = new Vector2(CustomMinimumSize.X, expectChildRect[^1].End.Y + focusChild.Size.Y + separation);
			else if (!isUsingProcess)
				CustomMinimumSize = new Vector2(CustomMinimumSize.X, lastChild.GetRect().End.Y);
		}
		else
		{
			if (isUsingProcess && dropZoneIndex == children.Length)
				CustomMinimumSize = new Vector2(expectChildRect[^1].End.X + focusChild.Size.X + separation, CustomMinimumSize.Y);
			else if (!isUsingProcess)
				CustomMinimumSize = new Vector2(lastChild.GetRect().End.X, CustomMinimumSize.Y);
		}

		// Adjust rect every process frame until child is dropped && finished lerping 
		// ( return to adjust when sort_children signal is emitted)
		if (!isAnimating && focusChild == null)
			isUsingProcess = false;
	}


	private void AdjustDropZoneRect()
	{
		dropZones.Clear();
		var children = GetVisibleChildren();
		for (int i = 0; i < children.Length; i++)
		{
			Rect2 dropZoneRect = new();
			var child = children[i] as Control;
			if (CustomMinimumSize.X == 0)
			{
				if (i == 0)
				{
					// First child
					dropZoneRect.Position = new Vector2(child.Position.X, child.Position.Y - dropZoneExtend);
					dropZoneRect.End = new Vector2(child.Size.X, child.GetRect().GetCenter().Y);
					dropZones.Add(dropZoneRect);
				}
				else
				{
					// In between
					var prevChild = children[i - 1] as Control;
					dropZoneRect.Position = new Vector2(prevChild.Position.X, prevChild.GetRect().GetCenter().Y);
					dropZoneRect.End = new Vector2(child.Size.X, child.GetRect().GetCenter().Y);
					dropZones.Add(dropZoneRect);
				}

				if (i == children.Length - 1)
				{
					// Is also last child
					dropZoneRect.Position = new Vector2(child.Position.X, child.GetRect().GetCenter().Y);
					dropZoneRect.End = new Vector2(child.Size.X, child.GetRect().End.Y + dropZoneExtend);
					dropZones.Add(dropZoneRect);
				}
			}
			else
			{
				if (i == 0)
				{
					// First child
					dropZoneRect.Position = new Vector2(child.Position.X - dropZoneExtend, child.Position.Y);
					dropZoneRect.End = new Vector2(child.GetRect().GetCenter().X, child.Size.Y);
					dropZones.Add(dropZoneRect);
				}
				else
				{
					// In between
					var prevChild = children[i - 1] as Control;
					dropZoneRect.Position = new Vector2(prevChild.GetRect().GetCenter().X, prevChild.Position.Y);
					dropZoneRect.End = new Vector2(child.GetRect().GetCenter().X, child.Size.Y);
					dropZones.Add(dropZoneRect);
				}
				
				if (i == children.Length - 1)
				{
					// Is also last child
					dropZoneRect.Position = new Vector2(child.GetRect().GetCenter().X, child.Position.Y);
					dropZoneRect.End = new Vector2(child.GetRect().End.X + dropZoneExtend, child.Size.Y);
					dropZones.Add(dropZoneRect);
				}
			}
		}
	}

	private Control[]  GetVisibleChildren()
	{
		List<Control> visibleControls = new();
		foreach (var _child in GetChildren())
		{
			if (_child is not Control child || !child.Visible || (child == focusChild && isHold))
				continue;

			visibleControls.Add(child);
		}
		
		return visibleControls.ToArray();
	}

	private void PrintDebug(string val)
	{
		if (isDebugging)
			GD.Print(val);
	}
}
