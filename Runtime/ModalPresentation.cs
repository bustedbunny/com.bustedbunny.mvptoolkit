using Unity.Assertions;

namespace MVPToolkit
{
    public abstract class ModalPresentation : BasePresentation
    {
        public override void Enable()
        {
            Assert.IsFalse(UISingleton.activeModals.Contains(this));
            UISingleton.activeModals.Add(this);
            base.Enable();
        }

        public override void Disable()
        {
            UISingleton.activeModals.Remove(this);
            base.Disable();
        }
    }
}