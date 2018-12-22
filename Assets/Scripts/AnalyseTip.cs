using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnalyseCards {
    class AnalyseTip {
        public byte[] GetTipDatas(byte[] datas, TypeInfo typeInfo) {
            typeInfo = info;
            Utility.PrepareDatas(datas, out infos);
            SetInfos();
        }

        bool IsOnlyType(CardInfo info, CardType type) {
            if ((info.type | type) == type) {
                return true;
            }
            return false;
        }
        bool IsType(CardInfo info, CardType type) {
            if ((info.type & type) == type) {
                return true;
            }
            return false;
        }
        List<List<byte>> tipDatas = new List<List<byte>>();
        TypeOnlyInfo typeOnlyInfo = new TypeOnlyInfo();
        SortedDictionary<CardValue, CardInfo> infos = null;
        TypeInfo typeInfo = null;
        void SetInfos() {
            SetDoubleThreeBombInfos();
            SetSequenceInfos(CardType.sequence);
            SetSequenceInfos(CardType.sequencePair);
            SetSequenceInfos(CardType.sequenceThree);
        }
        // 对牌，三张，炸弹牌型分析
        void SetDoubleThreeBombInfos() {
            foreach (KeyValuePair<CardValue, CardInfo> item in infos) {
                switch (item.Value.datas.Count) {
                    case ConstData.pairRequireCount:
                        item.Value.type |= CardType.pair;
                        break;
                    case ConstData.threeRequireCount:
                        item.Value.type |= CardType.pair;
                        item.Value.type |= CardType.three;
                        break;
                    case ConstData.bombRequireCount:
                        item.Value.type |= CardType.pair;
                        item.Value.type |= CardType.three;
                        item.Value.type |= CardType.bomb;
                        break;
                }
            }
        }
        // 顺子，连队，飞机数据分析
        void SetSequenceInfos(CardType type) {
            CardValue sequenceStart, sequenceEnd;
            CardValue startValue = ConstData.minCardValue;
            bool isHaveSequence = false;
            do {
                isHaveSequence = Utility.GetSequenceInfo(infos, type, startValue, out sequenceStart, out sequenceEnd);
                if (isHaveSequence) {
                    SetSequenceInfo(type, sequenceStart, sequenceEnd);
                    startValue = sequenceEnd + 1;
                }
            } while (isHaveSequence);
        }
        void SetSequenceInfo(CardType type, CardValue start, CardValue end) {
            for (CardValue i = start; i <= end; i++) {
                switch (type) {
                    case CardType.sequence:
                        infos[i].type |= CardType.sequence;
                        infos[i].SequenceStart = start;
                        infos[i].SequenceEnd = end;
                        break;
                    case CardType.sequencePair:
                        infos[i].type |= CardType.sequencePair;
                        infos[i].PairSequenceStart = start;
                        infos[i].PairSequenceEnd = end;
                        break;
                    case CardType.sequenceThree:
                        infos[i].type |= CardType.sequenceThree;
                        infos[i].ThreeSequenceStart = start;
                        infos[i].ThreeSequenceEnd = end;
                        break;

                }
            }
        }
        public override string ToString() {
            string str = string.Empty;
            foreach (KeyValuePair<CardValue, CardInfo> item in infos) {
                str += "\r\n card value:\t" + item.Key + "\r\n";
                str += item.Value.ToString();
            }
            return str;
        }
    }

}