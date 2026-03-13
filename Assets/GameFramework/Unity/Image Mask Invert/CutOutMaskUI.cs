using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
namespace IMax.Helper
{
    public class CutOutMaskUI : Image
    {
        public override Material materialForRendering
        {
            get
            {
                Material mat = new Material(base.materialForRendering);
                mat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return mat;
            }
        }
    }
}