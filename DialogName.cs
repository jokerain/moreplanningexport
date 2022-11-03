using Verse;
using MorePlanningExport.Data;

namespace MorePlanningExport
{
    public class DialogName : Dialog_Rename
    {
        private readonly MorePlanningData _morePlanning;
        
        private readonly Confirm _confirm;

        public delegate void Confirm();
        
        private readonly Cancel _cancel;

        public delegate void Cancel();

        public DialogName(MorePlanningData morePlanning, Confirm confirm, Cancel cancel)
        {
            _morePlanning = morePlanning;
            curName = morePlanning.Name;
            _confirm = confirm;
            _cancel = cancel;
        }

        protected override int MaxNameLength => 24;

        protected override AcceptanceReport NameIsValid(string newName)
        {
            if (newName == _morePlanning.Name)
            {
                closeOnAccept = true;
                return true;
            }
            var acceptanceReport = MorePlanningData.IsValidMorePlanningName(newName);
            if (!acceptanceReport.Accepted)
                return acceptanceReport;
            closeOnAccept = true;
            return AcceptanceReport.WasAccepted;
        }

        protected override void SetName(string name) => _morePlanning.Name = name;

        public override void PostClose()
        {
            base.PostClose();
            if (closeOnAccept)
            {
                _confirm();
            }
            else
            {
                _cancel();
            }
        }
    }
}