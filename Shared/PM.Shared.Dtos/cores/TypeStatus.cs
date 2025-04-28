using System.ComponentModel;

namespace PM.Shared.Dtos.cores
{
    public enum TypeStatus
    {
        Node,

        [Description("Not Selected")]
        NotSelected,

        [Description("Waiting")]
        Waiting,

        [Description("In Progress")]
        InProgress,

        [Description("Completed Early")]
        CompletedEarly,

        [Description("Finished On Time")]
        FinishedOnTime,

        [Description("Behind Schedule")]
        BehindSchedule,

        [Description("Finished Late")]
        FinishedLate
    }
}
