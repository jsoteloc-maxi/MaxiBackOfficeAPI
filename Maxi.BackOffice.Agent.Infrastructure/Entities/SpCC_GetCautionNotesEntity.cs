namespace Maxi.BackOffice.Agent.Infrastructure.Entities
{
    public class SpCC_GetCautionNotesEntity
    {
        public string Src { get; set; }
        public string Status { get; set; }
        public string Text { get; set; }
        public DateTime? ReviewDate { get; set; }


        public string Source { get => Src; set { Src = value; } } //solo para que coincida con el mapper
        public string SourceText { get; set; }
        public string TextTag { get; set; }
    }
}
