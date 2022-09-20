using System.ComponentModel.DataAnnotations;

namespace HowToWebApiAuthCode.Blazor.Models
{
    public class FormModel
    {
        public OwnerAccount[] Owners { get; set; }

        public BoundLock[] BoundLocks { get; set; }

        [Required]
        public string ContactEmail { get; set; }

        public string Message { get; set; }

        [Required]
        public string SelectedOwnerAccount { get; set; }

        [Required]
        public string SelectedBoundLock { get; set; }

    }
}
