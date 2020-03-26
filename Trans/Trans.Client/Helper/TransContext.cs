using System;
using System.Collections.Generic;
using System.Text;

namespace Trans.Client.Helper
{
    public enum LangType
    {
        en,
        zh,
        jp,
        zh_tw,
    }

    public enum TransStrategy
    {
        Baidu,
        Tencent,
        Custom,
        Google,
        Microsoft,
    }
    public interface ITransContext
    {
        public TransStrategy Strategy { get; set; }
        public ITrans GetTrans();
    }
    public class TransContext : ITransContext
    {
        public TransContext(ITrans trans)
        {
            Trans = trans;
        }
        private ITrans _trans;
        protected ITrans Trans 
        { 
            get
            {
                return _trans;
            }
            set
            {
                _trans = value;
            }
        }
        public TransStrategy Strategy { get; set; }
        public ITrans GetTrans()
        {
            if (Trans.Strategy == Strategy)
                return Trans;
            switch (Strategy) {
                case TransStrategy.Baidu:
                    Trans = new BaiduTrans();
                    break;
                case TransStrategy.Tencent:
                    Trans = new TencentTrans();
                    break;
                case TransStrategy.Google:
                    Trans = new GoogleTrans();
                    break;
                case TransStrategy.Microsoft:
                    Trans = new MicrosoftTrans();
                    break;
                case TransStrategy.Custom:
                    Trans = new CustomTrans();
                    break;
                default:
                    Trans = new BaiduTrans();
                    break;
            }
            return Trans;
        }
    }
}
