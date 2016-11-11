namespace DrinkOBand.Core.Helpers
{
    public interface IUnitHelper
    {
        int OZToMl(int value);
        int GetMappedAmount(int key);
        int GetAmount(int value);
        string AmountText { get; }
    }
}