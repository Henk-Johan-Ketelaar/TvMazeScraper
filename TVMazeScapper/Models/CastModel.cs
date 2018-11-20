using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TVMazeScapper.Models
{
    public class CastModel
    {
        public PersonDetail Person { get; set; }
    }

    public class PersonDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Birthday { get; set; }
    }
}