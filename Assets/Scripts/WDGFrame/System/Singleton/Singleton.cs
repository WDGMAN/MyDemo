using System;
using System.Reflection;

namespace WDGFrame
{
  public abstract  class Singleton<T> where T:class
    {
        private static T instance;
        // protected BaseSingleton(){}

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Type type = typeof(T);
                  ConstructorInfo info= type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                 instance= info.Invoke(null) as T;
                }
              
                return instance;
            }
        }

      

    }
  


}