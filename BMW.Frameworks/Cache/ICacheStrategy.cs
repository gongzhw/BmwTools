using System;
using System.Text;

namespace BMW.Frameworks.Cache
{
    /// <summary>
    /// ����������Խӿ�
    /// </summary>
    public interface ICacheStrategy
    {
        /// <summary>
        /// ���ָ��ID�Ķ���
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="o"></param>
        void AddObject(string objId, object o);

        /// <summary>
        /// ���ָ��ID�Ķ���(����ָ���ļ���)
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="o"></param>
        /// <param name="files"></param>
        void AddObjectWithFileChange(string objId, object o, string[] files);

        /// <summary>
        /// ���ָ��ID�Ķ���(����ָ����ֵ��)
        /// </summary>
        /// <param name="objId"></param>
        /// <param name="o"></param>
        /// <param name="dependKey"></param>
        void AddObjectWithDepend(string objId, object o, string[] dependKey);

        /// <summary>
        /// �Ƴ�ָ��ID�Ķ���
        /// </summary>
        /// <param name="objId"></param>
        void RemoveObject(string objId);

        /// <summary>
        /// ����ָ��ID�Ķ���
        /// </summary>
        /// <param name="objId"></param>
        /// <returns></returns>
        object getCacheObj(string objId);

        T getCacheObj<T>(string objId) where T : class;

        /// <summary>
        /// ����ʱ��
        /// </summary>
        int TimeOut { set;get;}
   }
}
