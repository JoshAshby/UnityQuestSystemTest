public abstract class FSMBaseState<T> {
    public virtual void DoBeforeEntering () { }
    public virtual void DoBeforeLeaving () { }

    public abstract T Reason ();
    public abstract void Act ();
}