//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Notestash_Admin
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblNote
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblNote()
        {
            this.tblCategories = new HashSet<tblCategory>();
            this.tblTemplates = new HashSet<tblTemplate>();
            this.tblDislikes = new HashSet<tblDislike>();
            this.tblLikes = new HashSet<tblLike>();
            this.tblComments = new HashSet<tblComment>();
        }
    
        public int Id { get; set; }
        public string NoteName { get; set; }
        public byte[] NoteContent { get; set; }
        public string Status { get; set; }
        public int DownloadNumbers { get; set; }
        public int LikeNumbers { get; set; }
        public int CommentNumbers { get; set; }
        public System.DateTime DateTime { get; set; }
        public int UserId { get; set; }
        public int DislikeNumbers { get; set; }
    
        public virtual tblUser tblUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblCategory> tblCategories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblTemplate> tblTemplates { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblDislike> tblDislikes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblLike> tblLikes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblComment> tblComments { get; set; }
    }
}
