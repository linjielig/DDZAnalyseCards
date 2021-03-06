﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnalyseCards {
    interface IAnalyse {
        bool IsWithRule(byte[] datas);
        bool IsGreater(byte[] datas1, byte[] datas2);
        byte[] GetTipDatas(byte[] datas);
    }
    class Analyse {
        static Analyse instance;
        public static Analyse Instance {
            get {
                if (instance == null) {
                    instance = new Analyse();
                }
                return instance;
            }
        }
        public string analyseTypeInfo;
        public string analyseTipInfo;
        AnalyseType analyseType = new AnalyseType();
        AnalyseTip analyseTip = new AnalyseTip();
        public bool IsWithRule(byte[] byteDatas) {
            TypeInfo typeInfo = analyseType.GetTypeInfo(byteDatas);
            analyseTypeInfo = analyseType.ToString();
            if (Utility.IsContainType(typeInfo, CardType.none)) {
                return false;
            }
            return true;
        }
        //public bool IsGreater(byte[] datas1, datas2) {

        //}
        public byte[] GetTipDatas(byte[] byteDatasHave, byte[] byteDatasType, bool isReadCache = true) {
            TypeInfo typeInfo = analyseType.GetTypeInfo(byteDatasType);
            if (typeInfo.type == CardType.none) {
                TypeInfo typeInfoHave = analyseType.GetTypeInfo(byteDatasHave);
                if (typeInfoHave.type != CardType.none) {
                    return byteDatasHave;
                }
            }
            byte[] tipDatas = analyseTip.GetTipDatas(byteDatasHave, typeInfo, isReadCache);
            analyseTipInfo = analyseTip.ToString();
            return tipDatas;
        }
    }
}

