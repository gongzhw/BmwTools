using System;
using System.Web;
using System.Collections;
using System.Web.Caching;

namespace BMW.Frameworks.Cache
{
    /// <summary>
    /// 缓存管理类
    /// </summary>
    public class CacheStrategy : ICacheStrategy
    {
        private static CacheStrategy _instance = null;
        private static object lockHelper = new object();

        protected static volatile System.Web.Caching.Cache objCache = System.Web.HttpRuntime.Cache;

        protected int _timeOut = 1440; // 默认缓存存活期为1440分钟(24小时)

        private static object syncObj = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        static CacheStrategy()
        {

        }

        /// <summary>
        /// Getthe instance of CS 
        /// </summary>
        public static CacheStrategy GetInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                    {
                        _instance = new CacheStrategy();
                    }
                }
            }
            return _instance;
        }

        /// <summary>
        /// 设置到期相对时间[单位：／分钟] 
        /// </summary>
        public int TimeOut
        {
            set { _timeOut = value > 0 ? value : 6000; }
            get { return _timeOut > 0 ? _timeOut : 6000; }
        }

        /// <summary>
        /// 加入当前对象到缓存中
        /// </summary>
        /// <param name="objId">对象的键值</param>
        /// <param name="o">缓存的对象</param>
        public void AddObject(string objId, object o)
        {

            if (objId == null || objId.Length == 0 || o == null)
            {
                return;
            }

            CacheItemRemovedCallback callBack = new CacheItemRemovedCallback(onRemove);

            if (TimeOut == 6000)
            {
                objCache.Insert(objId, o, null, DateTime.MaxValue, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, callBack);
            }
            else
            {
                objCache.Insert(objId, o, null, DateTime.Now.AddMinutes(TimeOut), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, callBack);
            }
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="objId">对象的键值</param>
        /// <param name="objObject">缓存的对象</param>
        /// <param name="absoluteExpiration">过期时间</param>
        /// <param name="slidingExpiration"></param>        
        public void AddObject(string objId, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            if (objId == null || objId.Length == 0 || objObject == null)
            {
                return;
            }
            CacheItemRemovedCallback callBack = new CacheItemRemovedCallback(onRemove);
            objCache.Insert(objId, objObject, null, absoluteExpiration, slidingExpiration, System.Web.Caching.CacheItemPriority.High, callBack);
        }

        /// <summary>
        /// 加入当前对象到缓存中
        /// </summary>
        /// <param name="objId">对象的键值</param>
        /// <param name="o">缓存的对象</param>
        public void AddObjectWith(string objId, object o)
        {
            if (objId == null || objId.Length == 0 || o == null)
            {
                return;
            }

            CacheItemRemovedCallback callBack = new CacheItemRemovedCallback(onRemove);

            objCache.Insert(objId, o, null, System.DateTime.Now.AddHours(TimeOut), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, callBack);
        }


        /// <summary>
        /// 加入当前对象到缓存中,并对相关文件建立依赖
        /// </summary>
        /// <param name="objId">对象的键值</param>
        /// <param name="o">缓存的对象</param>
        /// <param name="files">监视的路径文件</param>
        public void AddObjectWithFileChange(string objId, object o, string[] files)
        {
            if (objId == null || objId.Length == 0 || o == null)
            {
                return;
            }

            CacheItemRemovedCallback callBack = new CacheItemRemovedCallback(onRemove);

            CacheDependency dep = new CacheDependency(files, DateTime.Now);

            objCache.Insert(objId, o, dep, System.DateTime.Now.AddHours(TimeOut), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, callBack);
        }


        /// <summary>
        /// 加入当前对象到缓存中,并使用依赖键
        /// </summary>
        /// <param name="objId">对象的键值</param>
        /// <param name="o">缓存的对象</param>
        /// <param name="dependKey">依赖关联的键值</param>
        public void AddObjectWithDepend(string objId, object o, string[] dependKey)
        {
            if (objId == null || objId.Length == 0 || o == null)
            {
                return;
            }

            CacheItemRemovedCallback callBack = new CacheItemRemovedCallback(onRemove);

            CacheDependency dep = new CacheDependency(null, dependKey, DateTime.Now);

            objCache.Insert(objId, o, dep, System.DateTime.Now.AddMinutes(TimeOut), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, callBack);
        }


        /// <summary>
        /// 建立回调委托的一个实例
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="reason"></param>
        public void onRemove(string key, object val, CacheItemRemovedReason reason)
        {


            switch (reason)
            {
                case CacheItemRemovedReason.DependencyChanged:
                    break;
                case CacheItemRemovedReason.Expired:
                    {
                        break;
                    }
                case CacheItemRemovedReason.Removed:
                    {
                        break;
                    }
                case CacheItemRemovedReason.Underused:
                    {
                        break;
                    }
                default: break;
            }
        }


        /// <summary>
        /// 删除缓存对象
        /// </summary>
        /// <param name="objId">对象的关键字</param>
        public void RemoveObject(string objId)
        {
            try
            {
                if (objId == null || objId.Length == 0)
                {
                    return;
                }
                objCache.Remove(objId);
                //log.Info("从cache中移除: " + objId + " 对象");
            }
            catch (Exception ex)
            {
            }
        }


        /// <summary>
        /// 返回一个指定的对象
        /// </summary>
        /// <param name="objId">对象的关键字</param>
        /// <returns>对象</returns>
        public object getCacheObj(string objId)
        {
            return string.IsNullOrEmpty(objId) ? null : objCache.Get(objId);
        }

        public T getCacheObj<T>(string objId) where T : class
        {
            return getCacheObj(objId) as T;
        }
    }
}
