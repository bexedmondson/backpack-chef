public static class OrderStateExtensions
{
    public static bool IsEnded(this OrderState orderState)
    {
        return orderState is OrderState.Expired or OrderState.Complete;
    }

    public static bool IsFailed(this OrderState orderState)
    {
        return orderState is OrderState.FailedStep or OrderState.FailedBinned or OrderState.Expired;
    }
}
