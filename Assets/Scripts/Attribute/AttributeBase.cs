

    using UnityEngine;

    /// <summary>
    /// 属性类别
    /// </summary>
    public enum AttributeSpecies
    {
        Fire, //火
        Water, //水
        Normal,//普通
    }
    public abstract class AttributeBase:MonoBehaviour
    {
        public AttributeSpecies Species { get; private set; } //属性
        public int EvadeProbability { get; private set; } //闪避概率
        public int Lv { get; private set; } //等级
        #region 基础属性（白字）
        public int Health { get; private set; }
        public int Mp { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public float Speed { get;  set; }
        
        #endregion

        #region 实际面板属性(经过各种加成)

        public int HealthMax { get; private set; }
        public int MpMax { get; private set; }
        public int AttackMax { get; private set; }
        public int DefenseMax { get; private set; }
        public float SpeedMax { get; private set; }

        #endregion

        #region 临时属性 当前的属性
        
        public int HealthCurrent { get; private set; }
        public int MpCurrent { get; private set; }
        public int AttackCurrent { get; private set; }
        public int DefenseCurrent { get; private set; }
        public float SpeedCurrent { get;  set; }



       
        /// <summary>
        /// 初始化基础属性
        /// </summary>
        /// <param name="health"></param>
        /// <param name="mp"></param>
        /// <param name="attack"></param>
        /// <param name="defense"></param>
        public virtual void Init(int health,int mp,int attack,int defense,int speed,AttributeSpecies species)
        {
            Health = health;
            Mp = mp;
            Attack = attack;
            Defense = Defense;
            Species = species;
            Speed = speed;
            Lv = 1;

            CalculateMaxAttribute();
            CalculateCurrentAttribute();
        }

        /// <summary>
        /// 计算最终属性（实际最终属性）
        /// </summary>
        public void CalculateMaxAttribute()
        {
            HealthMax = Health;
            MpMax = Mp;
            AttackMax = Attack;
            DefenseMax = Defense;
            SpeedMax = Speed;
        }    
        
        /// <summary>
        /// 计算当前属性(buff影响等等)
        /// </summary>
        public void CalculateCurrentAttribute()
        {
            HealthCurrent = Health;
            MpCurrent = Mp;
            AttackCurrent = Attack;
            DefenseCurrent = Defense;
            SpeedCurrent = Speed;
        }

        
        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="val"></param>
        public void Hurt(int val)
        {
            HealthCurrent -= val;
            if (HealthCurrent < 0)
            {
                HealthCurrent = 0;
            }
        }

        /// <summary>
        /// 恢复生命值
        /// </summary>
        public void RecoverHealth(int val)
        {
            HealthCurrent += val;
            if (HealthCurrent > HealthMax) HealthCurrent = HealthMax;
        }
        
        
        #endregion
        
        
    }
