using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BMW.Frameworks.JsonHelper
{
    /// <summary>
    /// 自定义Json序列化规则
    /// </summary>
    public class JsonContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 显示指定需要序列化的属性（对象）的名称数组
        /// </summary>
        private string[] exceptMemberName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="exceptMemberName"></param>
        public JsonContractResolver(string[] exceptMemberName)
        {
            this.exceptMemberName = exceptMemberName;
        }

        /// <summary>
        /// 重写Newtonsoft的GetSerializableMembers 方法
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            //把给定的ilist数据所有属性都find到
            var members = new List<PropertyInfo>(objectType.GetProperties());
            //
            members.RemoveAll(memberInfo =>
                (IsInheritedISet(memberInfo)) ||
                (IsInheritedEntity(memberInfo))
                );

            var actualMemberInfos = new List<MemberInfo>();
            foreach (var memberInfo in members)
            {
                var infos = memberInfo.DeclaringType.BaseType.GetMember(memberInfo.Name);
                actualMemberInfos.Add(infos.Length == 0 ? memberInfo : infos[0]);
                //Debug.WriteLine(memberInfo.Name);
            }
            return actualMemberInfos;
        }

        /// <summary>
        /// 是否包含需要的成员（对象）
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private bool IsExceptMember(PropertyInfo memberInfo)
        {
            if (exceptMemberName == null)
                return false;

            bool isExceptMember = Array.Exists(exceptMemberName, i => memberInfo.Name == i);
            return isExceptMember;
        }

        /// <summary>
        /// 忽略掉数据类型为ICollection<T>的属性（多对一关系）
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private bool IsInheritedISet(PropertyInfo memberInfo)
        {
            bool isInheritedISet = memberInfo.PropertyType.Name == "ICollection`1";
            return isInheritedISet;
        }

        /// <summary>
        /// 忽略掉数据类型为Object的属性，传进来的例外字段名除外(EF中一对一是：public virtual 多对一是：public virtual ICollection)
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private bool IsInheritedEntity(PropertyInfo memberInfo)
        {
            bool isInheritedEntity = FindBaseType(memberInfo.PropertyType).Name == "Object" && !IsExceptMember(memberInfo);
            return isInheritedEntity;
        }

        /// <summary>
        /// 获取属性的类型 如果是Object 代表是一个(一对多 OR 多对一的对象)
        /// </summary>
        /// <param name="type">type类型</param>
        /// <returns></returns>
        private static Type FindBaseType(Type type)
        {
            if (!type.IsClass)
                return type;
            if (type.Name == "Object")
            {
                return FindBaseType(type.BaseType);
            }
            else
            {
                return type;
            }

        }
    }

}
