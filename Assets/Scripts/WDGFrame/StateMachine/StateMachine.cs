using System;
using System.Collections.Generic;
using UnityEngine;

namespace WDGFrame
{
    public class StateMachine<T>
    {
        public StateMachine<T> ParentFSM { get; private set; } //父层状态
        public GameObject Owner { get; private set; } //游戏对象

        private Dictionary<T, StateBase<T>> StateDic = new Dictionary<T, StateBase<T>>();

        //保存的上一個狀態
        public StateBase<T> LastState { get; private set; }

        //保存的上一個狀態id
        public T LastStateId { get; private set; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="ownerEnum"></param>
        public StateMachine()
        {
        }

        /// <summary>
        /// 拥有父状态机
        /// </summary>
        /// <param name="parentFsm"></param>
        public StateMachine(StateMachine<T> parentFsm)
        {
            ParentFSM = parentFsm;
        }

        /// <summary>
        /// 当前状态Id
        /// </summary>
        public T CurrentStateId { get; private set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public StateBase<T> CurrentState { get; private set; }


        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="stateBase"></param>
        public void AddState(T stateId, StateBase<T> stateBase)
        {
            if (StateDic.ContainsKey(stateId))
            {
                Debug.Log("已经存在该状态,添加失败");
            }
            else
            {
                StateDic.Add(stateId, stateBase);
            }
        }

        public void RemoveState(T stateId)
        {
            if (StateDic.ContainsKey(stateId))
            {
                StateDic.Remove(stateId);
            }
            else
            {
                Debug.Log("未找到该状态,移除失败");
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="stateId"></param>
        public void ChangeState(T stateId)
        {
            if (StateDic.TryGetValue(stateId, out StateBase<T> stateBase))
            {
                if (CurrentState != null)
                {
                    CurrentState.Exit();
                    //保存上一個狀態信息
                    LastState = CurrentState;
                    LastStateId = CurrentStateId;
                }

                CurrentStateId = stateId;
                CurrentState = stateBase;
                CurrentState.Enter();
            }
            else
            {
                Debug.Log("未找到目标状态,切换失败");
            }
        }


        /// <summary>
        /// 切换状态 
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="reset">默认true表示可转换相同状态</param>
        public void ChangeState(T stateId, bool reset)
        {
            if (!reset)
            {
                Debug.Log("该状态与转换状态相同，转换失败");
            }
            else
            {
                ChangeState(stateId);
            }
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        public StateBase<T> GetState(T stateId)
        {
            if (StateDic.TryGetValue(stateId, out StateBase<T> stateBase))
            {
                return stateBase;
            }

            return null;
        }

        public void OnUpdate()
        {
            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }

        public void OnLateUpdate()
        {
            if (CurrentState != null) CurrentState.LateUpdate();
        }

        public void OnFixedUpdate()
        {
            if (CurrentState != null) CurrentState.FixedUpdate();
        }

        #region 共享数据

        private Dictionary<string, object> sharedDic = new Dictionary<string, object>();

        /// <summary>
        /// 添加共享数据
        /// </summary>
        public void AddShareData(string name, object val=null)
        {
            if (sharedDic.ContainsKey(name)) Debug.Log("已有该数据 无法添加");
            else
            {
                sharedDic.Add(name, val); 
            }
        }

        /// <summary>
        /// 获取共享数据
        /// </summary>
        public T GetShareData<T>(string name)
        {
            if (sharedDic.ContainsKey(name))
            {
                return (T)sharedDic[name];
            }
            else
            {
                
                Debug.Log("获取失败 没有该数据"+sharedDic.Count); 
                return default(T);
                
            }
        }

        /// <summary>
        /// 修改共享数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        public void SetShareData(string name, object val)
        {
            if (sharedDic.ContainsKey(name))
            {
                sharedDic[name] = val;
            }
            else Debug.Log("修改失败,无该数据");
        }
        #endregion
    }
}