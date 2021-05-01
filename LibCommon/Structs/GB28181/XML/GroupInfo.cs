using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LibCommon.Structs.GB28181.XML
{
    /// <summary>
    /// 组织结构
    /// </summary>
    [XmlRoot("GroupInfo")]
    public class GroupInfo : XmlHelper<GroupInfo>, IDisposable
    {
        private static GroupInfo instance;

        private List<GroupInfoItem> mGroupInfoItems = new List<GroupInfoItem>(500);

        /// <summary>
        /// 以单例模式访问
        /// </summary>
        public static GroupInfo Instance
        {
            get
            {
                if (instance == null)
                    instance = new GroupInfo();
                return instance;
            }
        }

        [XmlElement("GroupInfoItem")]
        public List<GroupInfoItem> Items
        {
            get { return this.mGroupInfoItems; }
        }

        public void Dispose()
        {
            Items.Clear();
            instance = null;
        }

        #region 子类

        public class GroupInfoItem
        {
            public GroupInfoItem()
            {
            }

            public GroupInfoItem(string groupCode, string name, string parentID, string groupType, string groupMark,
                string groupIdentify, string subordinatePlatform)
            {
                GroupCode = groupCode;
                Name = name;
                ParentID = parentID;
                GroupType = groupType;
                GroupMark = groupMark;
                GroupIdentify = groupIdentify;
                SubordinatePlatform = subordinatePlatform;
            }

            /// <summary>
            /// 索引
            /// </summary>
            [XmlAttribute("Guid")]
            public ushort Guid { get; set; }

            /// <summary>
            /// 分组编码
            /// </summary>
            [XmlAttribute("GroupCode")]
            public string GroupCode { get; set; }

            /// <summary>
            /// 分组名称
            /// </summary>
            [XmlAttribute("Name")]
            public string Name { get; set; }

            /// <summary>
            /// 分组上级ID
            /// </summary>
            [XmlAttribute("ParentID")]
            public string ParentID { get; set; }

            /// <summary>
            /// 虚拟分组ID
            /// </summary>
            [XmlAttribute("BusinessGroupID")]
            public string BusinessGroupID { get; set; }

            /// <summary>
            /// 分组类型
            /// </summary>
            [XmlAttribute("GroupType")]
            public string GroupType { get; set; }

            /// <summary>
            /// 分组备注名
            /// </summary>
            [XmlAttribute("GroupMark")]
            public string GroupMark { get; set; }

            /// <summary>
            /// 分组识别码
            /// 0，自建设备 
            /// 1，国标
            /// </summary>
            [XmlAttribute("GroupIdentify")]
            public string GroupIdentify { get; set; }

            /// <summary>
            /// 所属平台编码
            /// </summary>
            [XmlAttribute("SubordinatePlatform")]
            public string SubordinatePlatform { get; set; }

            /// <summary>
            /// 设备/区域/系统IP地址
            /// </summary>
            [XmlAttribute("IPAddress")]
            public string IPAddress { get; set; }

            /// <summary>
            /// 设备/区域/系统端口
            /// </summary>
            [XmlIgnore]
            public ushort? Port { get; set; }

            [XmlAttribute("Port")]
            public string PortValue
            {
                get { return Port.HasValue ? Port.Value.ToString() : null; }
                set
                {
                    ushort result;
                    Port = ushort.TryParse(value, out result) ? result : (ushort?) null;
                }
            }

            /// <summary>
            /// 注册方式(必选)缺省为1；
            /// 1:符合IETF FRC 3261标准的认证注册模式；
            /// 2:基于口令的双向认证注册模式；
            /// 3:基于数字证书的双向认证注册模式；
            /// </summary>
            [XmlIgnore]
            public int? RegisterWay { get; set; }

            [XmlAttribute("RegisterWay")]
            public string RegisterWayValue
            {
                get { return RegisterWay.HasValue ? RegisterWay.Value.ToString() : null; }
                set
                {
                    int result;
                    RegisterWay = int.TryParse(value, out result) ? result : (int?) null;
                }
            }

            /// <summary>
            /// 保密属性(必选)
            /// 0：不涉密
            /// 1涉密
            /// </summary>
            [XmlIgnore]
            public int? Secrecy { get; set; }

            [XmlAttribute("Secrecy")]
            public string SecrecyValue
            {
                get { return Secrecy.HasValue ? Secrecy.Value.ToString() : null; }
                set
                {
                    int result;
                    Secrecy = int.TryParse(value, out result) ? result : (int?) null;
                }
            }

            /// <summary>
            /// 设备状态(必选)
            /// </summary>
            [XmlAttribute("Status")]
            public string Status { get; set; }
        }

        #endregion

        #region 方法

        public void Save()
        {
            base.Save(this);
        }

        public void Read()
        {
            instance = this.Read(this.GetType());
        }

        public GroupInfoItem Get(string id)
        {
            foreach (var item in Instance.Items)
            {
                if (id == item.GroupCode.ToString())
                    return item;
            }

            return null;
        }

        public GroupInfoItem Get(int guid)
        {
            foreach (var item in Instance.Items)
            {
                if (guid == item.Guid)
                    return item;
            }

            return null;
        }

        public void Remove(string id)
        {
            var item = Items.Find(e => e.GroupCode.ToString() == id);
            Items.Remove(item);
        }

        private List<GroupInfoItem> list = new List<GroupInfoItem>();

        public void RemoveAll(string id)
        {
            list.Clear();

            var item = Items.FirstOrDefault(e => e.GroupCode == id);
            if (item == null)
                throw new Exception("文件中为找到相应节点！");

            list.Add(item);

            var items = Items.FindAll(e => e.ParentID == id);
            if (items.Count > 0)
            {
                foreach (var t in items)
                {
                    list.Add(t);
                    findChildren(t.GroupCode);
                }
            }

            foreach (var s in list)
            {
                Items.Remove(s);
            }
        }

        private void findChildren(string id)
        {
            var items = Items.FindAll(e => e.ParentID == id);
            if (items.Count > 0)
            {
                foreach (var t in items)
                {
                    list.Add(t);
                    findChildren(t.GroupCode);
                }
            }
        }

        public GroupInfoItem GetParent(string id)
        {
            GroupInfoItem parent = null;

            GroupInfoItem item = Get(id);
            if (item != null)
            {
                var p = Items.FirstOrDefault(e => e.ParentID == item.GroupCode);
                if (p != null)
                    parent = p;
            }

            return parent;
        }

        public void MatchParent()
        {
            if (Items.Count > 0)
            {
                foreach (var item in Items)
                {
                    if (item.GroupIdentify == "0")
                        continue;
                    if (string.IsNullOrEmpty(item.ParentID))
                    {
                        if (item.GroupCode == item.SubordinatePlatform)
                        {
                        }

                        if (item.GroupCode.Length == 2) //ProviceCata
                        {
                            var root = Items.FirstOrDefault(p => p.SubordinatePlatform == p.GroupCode);
                            if (root == null)
                                continue;

                            item.ParentID = root.GroupCode;
                        }
                        else if (item.GroupCode.Length == 4) //CityCata
                        {
                            var proviceCata = Items.FirstOrDefault(p => p.GroupCode == item.GroupCode.Substring(0, 2));
                            if (proviceCata == null)
                                continue;

                            item.ParentID = proviceCata.GroupCode;
                        }
                        else if (item.GroupCode.Length == 6) //AreaCata
                        {
                            var areaCata = Items.FirstOrDefault(p => p.GroupCode == item.GroupCode.Substring(0, 4));
                            if (areaCata == null)
                                continue;

                            item.ParentID = areaCata.GroupCode;
                        }
                        else if (item.GroupCode.Length == 8) //BasicUnit
                        {
                            var basicUnit = Items.FirstOrDefault(p => p.GroupCode == item.GroupCode.Substring(0, 6));
                            if (basicUnit == null)
                                continue;

                            item.ParentID = basicUnit.GroupCode;
                        }
                    }
                }

                Instance.Save();
            }
        }

        public ushort CreatGuid()
        {
            ushort Guid = 0;

            foreach (var guidItem in this.mGroupInfoItems)
            {
                if (guidItem.Guid >= Guid)
                    Guid = guidItem.Guid;
            }

            return (ushort) (Guid + 1);
        }

        public ushort MatchParent(ushort guid)
        {
            ushort parent = 0;

            var item = this.Get(guid);
            if (item != null)
            {
                var itemP = this.Get(item.ParentID);
                if (itemP != null)
                    parent = itemP.Guid;
            }

            return parent;
        }

        #endregion
    }
}