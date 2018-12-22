using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnalyseCards {
    interface IAnalyse {
        bool IsWithRule(byte[] datas);
        bool IsGreater(byte[] datas1, byte[] datas2);
        byte[] GetTipDatas(byte[] datas);
    }
    class Analyse : IAnalyse {
        AnalyseType analyseType = new AnalyseType();
        AnalyseTip analyseTip = new AnalyseTip();
        public bool IsWithRule(byte[] datas) {

        }
        public bool IsGreater(byte[] datas1, datas2) {

        }
        public byte[] GetTipDatas(byte[] datas) {

        }
    }
}

