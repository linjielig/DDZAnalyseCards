using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnalyseCards {
    class AnalyseTip {
        public byte[] GetTipDatas(byte[] byteDatas, TypeInfo typeInfo) {
            this.typeInfo = typeInfo;
            Utility.PrepareDatas(byteDatas, out infos);
            typeOnlyInfo = Utility.GetTypeOnlyInfo(infos);
            SetInfos();
        }

        void SetInfos() {
            SetDoubleThreeBombInfos();
            SetSequenceInfos(CardType.sequence);
            SetSequenceInfos(CardType.sequencePair);
            SetSequenceInfos(CardType.sequenceThree);
        }

        void GetSingleTipDatas() {
            if (typeOnlyInfo.singleKeys.Count > 0) {
                foreach (CardValue cardValue in typeOnlyInfo.singleKeys) {
                    if (cardValue > typeInfo.mainValue[0]) {
                        tipDatas.Add(infos[cardValue].byteDatas);
                    }
                }
            }
            //} else if (typeOnlyInfo.pairKeys.Count > 0) {
            //    foreach (CardValue cardValue in typeOnlyInfo.pairKeys) {
            //        if (cardValue > typeInfo.mainValue[0]) {
            //            tipDatas.Add(new List<byte> { infos[cardValue].byteDatas[0] });
            //        }
            //    }
            //}
            //if (typeOnlyInfo.bombKeys.Count > 0) {
            //    foreach (CardValue cardValue in typeOnlyInfo.bombKeys) {
            //            tipDatas.Add(infos[cardValue].byteDatas);
            //    }
            //}
            //if (tipDatas.Count < 1) {
            //    if (typeOnlyInfo.threeKeys.Count > 0) {
            //        foreach (CardValue cardValue in typeOnlyInfo.threeKeys) {
            //            if (cardValue > typeInfo.mainValue[0]) {
            //                tipDatas.Add(new List<byte> { infos[cardValue].byteDatas[0] });
            //            }
            //        }
            //    }
            //}
        }
        void GetPairTipDatas() {
            if (typeOnlyInfo.pairKeys.Count > 0) {
                foreach (CardValue cardValue in typeOnlyInfo.pairKeys) {
                    if (cardValue > typeInfo.mainValue[0]) {
                        tipDatas.Add(infos[cardValue].byteDatas);
                    }
                }
            }

        }
        void GetThreeTipDatas() {
            if (typeOnlyInfo.threeKeys.Count > 0) {
                foreach (CardValue cardValue in typeOnlyInfo.threeKeys) {
                    if (cardValue > typeInfo.mainValue[0]) {
                        tipDatas.Add(infos[cardValue].byteDatas);
                    }
                }
            }
        }
        void GetBombTipDatas() {
            if (typeOnlyInfo.bombKeys.Count > 0) {
                foreach (CardValue cardValue in typeOnlyInfo.bombKeys) {
                    if (cardValue > typeInfo.mainValue[0]) {
                        tipDatas.Add(infos[cardValue].byteDatas);
                    }
                }
            }
        }
        void GetRocketTipDatas() {
            if (infos.ContainsKey(CardValue.blackJoker) && infos.ContainsKey(CardValue.redJoker)) {
                tipDatas.Add(new List<byte> { infos[CardValue.blackJoker].byteDatas[0], infos[CardValue.redJoker].byteDatas[0] });
            }
        }
        void GetSequenceDatas(CardType type) {
            foreach (KeyValuePair<CardValue, TypeInfo> item in infos) {
                if (IsType(item.Value, type)) {
                    switch (type) {
                        case CardType.sequence:
                            if (item.Value.SequenceStart > typeInfo.mainValue[0] && item.Value.SequenceEnd > typeInfo.mainValue[1]) {
                                for (CardValue start = item.Value.SequenceStart; start < item.Value.SequenceEnd; start++) {

                                }
                            }
                            CardValue start = item.Value.SequenceStart;
                            while (item.Value.SequenceEnd - start >= typeInfo.SequenceCount )
                            break;
                        case CardType.sequencePair:
                            break;
                        case CardType.sequenceThree:
                            break;
                    }
                }
            }
        }
        bool IsType(TypeInfo info, CardType type) {
            if ((info.type & type) == type) {
                return true;
            }
            return false;
        }
        List<List<byte>> tipDatas = new List<List<byte>>();
        TypeOnlyInfo typeOnlyInfo;
        SortedDictionary<CardValue, TypeInfo> infos;
        TypeInfo typeInfo;

        // 对牌，三张，炸弹牌型分析
        void SetDoubleThreeBombInfos() {
            foreach (KeyValuePair<CardValue, TypeInfo> item in infos) {
                switch (item.Value.byteDatas.Count) {
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
            foreach (KeyValuePair<CardValue, TypeInfo> item in infos) {
                str += "\r\n card value:\t" + item.Key + "\r\n";
                str += item.Value.ToString();
            }
            return str;
        }
    }

}