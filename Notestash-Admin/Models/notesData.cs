using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notestash_Admin.Models
{
    public class notesData
    {
        public int Id { get; set; }
        public string NoteName { get; set; }
        public string NoteContent { get; set; }
        public string NoteStatus { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Comments { get; set; }
        public int Downloads { get; set; }
        public DateTime Created_At { get; set; }
    }
}