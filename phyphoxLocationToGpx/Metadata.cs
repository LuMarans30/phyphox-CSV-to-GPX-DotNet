using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV2GPX
{
    internal class Metadata
    {
        public string author { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string email { get; set; }
        public string url { get; set; }
        public string urlname { get; set; }
        public string time { get; set; }
        public string keywords { get; set; }
        public string bounds { get; set; }

        public Metadata(string author, string name, string desc, string email, string url, string urlname, string time, string keywords, string bounds): this()
        {
            this.author = author;
            this.name = name;
            this.desc = desc;
            this.email = email;
            this.url = url;
            this.urlname = urlname;
            this.time = time;
            this.keywords = keywords;
            this.bounds = bounds;
        }

        public Metadata()
        {
            author = "";
            name = "";
            desc = "";
            email = "";
            url = "";
            urlname = "";
            time = "";
            keywords = "";
            bounds = "";
        }

        public override string ToString()
        {
            string metadata = $@"<metadata>";

            //Foreach variable in the class

            foreach (var property in GetType().GetProperties())
            {
                if (!string.IsNullOrEmpty(property.GetValue(this)?.ToString()))
                {
                    metadata += $"\n\t<{property.Name}>{property.GetValue(this)}</{property.Name}>";
                }
            }

            return metadata + $"\n</metadata>";
        }
    }
}
