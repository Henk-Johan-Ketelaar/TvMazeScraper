using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TVMazeScapper.Models
{
    public class ComboModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<CastDetail> Cast { get; set; }
    }

    public class CastDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Birthday { get; set; }
    }
}