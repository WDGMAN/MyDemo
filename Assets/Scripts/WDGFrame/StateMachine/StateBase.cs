using System;

namespace WDGFrame
{
    public abstract class StateBase<T>
    {
        protected StateMachine<T> fsm; //状态机
        protected GameObjectController<T> controller;

        public StateBase(StateMachine<T>  fsm,GameObjectController<T> controller)
        {
            this.fsm = fsm;
            this.controller = controller;
        }

     

        public abstract void Enter(); //进入
        public abstract void Exit(); //离开
        
        public virtual void Update(){}//刷新
        public virtual void LateUpdate(){}//update刷新之后
        public virtual void FixedUpdate(){}//物理刷新
    }

}