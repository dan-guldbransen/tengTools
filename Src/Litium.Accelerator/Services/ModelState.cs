namespace Litium.Accelerator.Services
{
    public abstract class ModelState
    {
        public abstract void AddModelError(string key, string errorMessage);
        public abstract bool IsValid { get; }
    }
}
