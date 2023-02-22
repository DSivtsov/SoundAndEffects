using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Test
{
    public class TestClass
    {
        public void ProcessInfo(List<InfoBase> infoList)
        {
            StringBuilder result = new StringBuilder();

            for (var i = 0; i < infoList.Count; i++)
            {
                var info = infoList[i];
                if (this.Validate(info))
                {
                    var infoData = this.ExtractData(info);
                    result.Append(infoData);
                }
                else
                {
                    infoList.Remove(info);
                }
            }

            Debug.Log(result.ToString());
        }

        private bool Validate<T>(T infoBase) where T : InfoBase
        {
            bool isValid = false;

            switch (infoBase.GetInfoType())
            {
                case InfoType.Sport:
                    {
                        if (infoBase is InfoSport @info)
                        {
                            if (info.title.StartsWith("Sport news: "))
                            {
                                isValid = true;
                            }
                        }
                    }
                    break;
                case InfoType.Music:
                    {
                        if (infoBase is InfoMusic @info)
                        {
                            if (info.title.StartsWith("Music charts: "))
                            {
                                isValid = true;
                            }
                        }
                    }
                    break;
            }

            return infoBase != null && isValid;
        }

        private StringBuilder ExtractData(InfoBase info)
        {
            StringBuilder result = new StringBuilder();
            string title = info.GetInfoType().GetTitle();

            result.Append(title).Append("\r\n");
            result.Append(info.GetData()).Append("\r\n\r\n");

            return result;
        }

        public abstract class InfoBase
        {
            protected InfoType infoType;

            public InfoType GetInfoType()
            {
                return this.infoType;
            }

            public abstract string GetData();
        }

        public class InfoMusic : InfoBase
        {
            public string title;
            public string author;

            public InfoMusic(string title, string author)
            {
                this.infoType = InfoType.Music;
                this.title = title;
                this.author = author;
            }

            public override string GetData()
            {
                return $"{this.title}\r\nAuthor:{this.author}";
            }
        }

        public class InfoSport : InfoBase
        {
            public string title;
            public string data;

            public InfoSport(string title, string data)
            {
                this.infoType = InfoType.Sport;
                this.title = title;
                this.data = data;
            }

            public override string GetData()
            {
                return $"{this.title}\r\n{this.data}";
            }
        }

        private enum InfoType
        {
            Music,
            Sport,
        }
    }

    public static class InfoTypeExtensions
    {
        public static string GetTitle(this TestClass.InfoType value)
        {
            switch (value)
            {
                case TestClass.InfoType.Sport:
                    return "<b>Breaking sport news!</b>";
                case TestClass.InfoType.Music:
                    return "<b>New single!</b>";
            }

            return "";
        }
    }
}

