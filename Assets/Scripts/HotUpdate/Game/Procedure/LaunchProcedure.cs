using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LaunchProcedure : BaseProcedure
{
    public override async Task OnEnterProcedure(object value)
    {

        Debug.Log("ÇÐ»»Îª LaunchProcedure");
        await base.OnEnterProcedure(value);
    }
}
