namespace VertigoCase.Core
{
    /// <summary>
    /// Strategy for scaling reward amounts by zone — swappable for different curves.
    /// </summary>
    public interface IRewardScaling
    {
        int Scale(int baseAmount, int zone);
    }
}
