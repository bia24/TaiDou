using System;
using System.Collections.Generic;
using System.Text;

namespace TaidouCommon
{
    public enum ReturnCode
    {
        Exception,
        Success,
        Fail,
        EmptyRole,
        HasRole,
        GetTask,
        UpdateTask,
        GetKnapsack,
        UpdateKnapsack,
        EmptyKnapsack,
        UpdateRoleSuccess,
        EmptySkill,
        HasSkill,
        WaitTeam,
        CreateTeamSuccess,
        CancelTeamSuccess,
        EndGame
    }
}
