using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnalyseCards {
    class AnalyseTip : MonoBehaviour {
        byte tipIndex = 0;
        public byte[] GetTipDatas(byte[] byteDatas, TypeInfo typeInfo, bool isReadCache = true) {
            if (isReadCache) {
                tipIndex++;
            } else {
                tipIndex = 0;
                tipDatas.Clear();
                this.typeInfo = typeInfo;
                Utility.PrepareDatas(byteDatas, out infos);
                onlyTypeInfo = Utility.GetOnlyTypeInfo(infos);
                SetInfos();
                GetDatas();
            }
            if (tipDatas.Count < 1) {
                return null;
            }
            return tipDatas[tipIndex % tipDatas.Count].ToArray();
        }
        void GetDatas() {
            switch (typeInfo.type) {
                case CardType.single:
                    GetSingle();
                    SplitPairToSingle();
                    SplitThreeToSingle();
                    GetBombForOther();
                    GetRocketForOther();
                    break;
                case CardType.pair:
                    GetPair();
                    GetBombForOther();
                    SplitThreeToPair();
                    GetRocketForOther();
                    break;
                case CardType.three:
                    GetThree();
                    GetBombForOther();
                    GetRocketForOther();
                    break;
                case CardType.threeSingle:
                    GetThreeSingle();
                    SplitPairToThreeSingle();
                    GetBombForOther();
                    GetRocketForOther();
                    SplitThreeToThreeSingle();
                    break;
                case CardType.threePair:
                    GetThreePair();
                    GetBombForOther();
                    GetRocketForOther();
                    SplitThreeToThreePair();
                    break;
                case CardType.bombSingle:
                    GetBombForOther();
                    GetBombSingle();
                    break;
                case CardType.bombPair:
                    GetBombForOther();
                    GetBombPair();
                    break;
            }
        }
        void SetInfos() {
            SetDoubleThreeBombInfos();
            SetSequenceInfos(CardType.sequence);
            SetSequenceInfos(CardType.sequencePair);
            SetSequenceInfos(CardType.sequenceThree);
        }
        void GetSingle() {
            if (onlyTypeInfo[CardType.single].Count > 0) {
                foreach (CardValue cardValue in onlyTypeInfo[CardType.single]) {
                    if (cardValue > typeInfo.mainValue) {
                        tipDatas.Add(infos[cardValue].byteDatas);
                    }
                }
            }
        }
        void SplitPairToSingle() {
            if (tipDatas.Count > 0) {
                return;
            }
            if (onlyTypeInfo[CardType.pair].Count > 0) {
                foreach (CardValue cardValue in onlyTypeInfo[CardType.pair]) {
                    if (cardValue > typeInfo.mainValue) {
                        tipDatas.Add(new List<byte> { infos[cardValue].byteDatas[0] });
                    }
                }
            }
        }
        void SplitThreeToSingle() {
            if (tipDatas.Count > 0) {
                return;
            }
            if (onlyTypeInfo[CardType.three].Count > 0) {
                foreach (CardValue cardValue in onlyTypeInfo[CardType.three]) {
                    if (cardValue > typeInfo.mainValue) {
                        tipDatas.Add(new List<byte> { infos[cardValue].byteDatas[0] });
                    }
                }
            }
        }
        void GetPair() {
            if (onlyTypeInfo[CardType.pair].Count > 0) {
                foreach (CardValue cardValue in onlyTypeInfo[CardType.pair]) {
                    if (cardValue > typeInfo.mainValue) {
                        tipDatas.Add(infos[cardValue].byteDatas);
                    }
                }
            }
        }
        void SplitThreeToPair() {
            if (tipDatas.Count > 0) {
                return;
            }
            if (onlyTypeInfo[CardType.three].Count > 0) {
                foreach (CardValue cardValue in onlyTypeInfo[CardType.three]) {
                    if (cardValue > typeInfo.mainValue) {
                        tipDatas.Add(new List<byte> { infos[cardValue].byteDatas[0], infos[cardValue].byteDatas[1] });
                    }
                }
            }
        }
        void GetThree() {
            if (onlyTypeInfo[CardType.three].Count > 0) {
                foreach (CardValue cardValue in onlyTypeInfo[CardType.three]) {
                    if (cardValue > typeInfo.mainValue) {
                        tipDatas.Add(infos[cardValue].byteDatas);
                    }
                }
            }
        }
        bool GetSingleForThree(CardValue cardValue, List<byte> listByte) {
            if (onlyTypeInfo[CardType.single].Count > 0) {
                listByte.AddRange(infos[onlyTypeInfo[CardType.single][0]].byteDatas);
                return true;
            }
            return false;
        }
        void GetPairForThree() {

        }
        List<List<byte>> GetThreeSingleThree() {
            List<List<byte>> listThree = new List<List<byte>>();
            if (onlyTypeInfo[CardType.three].Count > 0) {
                foreach (CardValue cardValue in onlyTypeInfo[CardType.three]) {
                    if (cardValue > typeInfo.mainValue) {
                        listThree.Add(infos[cardValue].byteDatas);
                    }
                }
            }
            return listThree;
        }
        void GetThreeSingle() {
            List<List<byte>> listThree = GetThreeSingleThree();
            if (listThree.Count > 0) {
                foreach (List<byte> listByte in listThree) {
                    if (onlyTypeInfo[CardType.single].Count > 0) {
                        listByte.Add(infos[onlyTypeInfo[CardType.single][0]].byteDatas[0]);
                        tipDatas.Add(listByte);
                    }
                }
            }
        }
        void SplitPairToThreeSingle() {
            if (tipDatas.Count > 0) {
                return;
            }
            List<List<byte>> listThree = GetThreeSingleThree();
            if (listThree.Count > 0) {
                foreach (List<byte> listByte in listThree) {
                    if (onlyTypeInfo[CardType.pair].Count > 0) {
                        listByte.Add(infos[onlyTypeInfo[CardType.pair][0]].byteDatas[0]);
                        tipDatas.Add(listByte);
                    }
                }
            }
        }
        void SplitThreeToThreeSingle() {
            if (tipDatas.Count > 0) {
                return;
            }
            List<List<byte>> listThree = GetThreeSingleThree();
            if (listThree.Count > 0) {
                foreach (List<byte> listByte in listThree) {
                    if (onlyTypeInfo[CardType.three].Count > 1) {
                        foreach (CardValue value in onlyTypeInfo[CardType.three]) {
                            if (value != Utility.GetCardValue(listByte[0])) {
                                listByte.Add(infos[onlyTypeInfo[CardType.three][0]].byteDatas[0]);
                                tipDatas.Add(listByte);
                                break;
                            }
                        }

                    }
                }
            }
        }
        void GetThreePair() {
            List<List<byte>> listThree = GetThreeSingleThree();
            if (listThree.Count > 0 && onlyTypeInfo[CardType.pair].Count > 0) {
                foreach (List<byte> listByte in listThree) {
                    listByte.AddRange(infos[onlyTypeInfo[CardType.pair][0]].byteDatas);
                    tipDatas.Add(listByte);
                }
            }
        }
        void SplitThreeToThreePair() {
            if (tipDatas.Count > 0) {
                return;
            }
            List<List<byte>> listThree = GetThreeSingleThree();
            if (listThree.Count > 0 && onlyTypeInfo[CardType.three].Count > 1) {
                foreach (List<byte> listByte in listThree) {
                    foreach (CardValue value in onlyTypeInfo[CardType.three]) {
                        if (value != Utility.GetCardValue(listByte[0])) {
                            listByte.Add(infos[value].byteDatas[0]);
                            listByte.Add(infos[value].byteDatas[1]);
                            tipDatas.Add(listByte);
                        }
                    }
                }

            }
        }
        void GetBombForOther() {
            if (tipDatas.Count > 0) {
                return;
            }
            if (onlyTypeInfo[CardType.bomb].Count > 0) {
                foreach (CardValue cardValue in onlyTypeInfo[CardType.bomb]) {
                    tipDatas.Add(infos[cardValue].byteDatas);
                }
            }
        }
        void ShowTipDatas() {
            string str = "\r\ntipDatas数据\r\n";
            foreach (List<byte> listByte in tipDatas) {
                str += "\r\n提示:\t";
                foreach (byte b in listByte) {
                    str += b + ",\t";
                }
                Debug.LogError(str + "\r\n");
            }
        }
        void GetBomb() {
            if (onlyTypeInfo[CardType.bomb].Count > 0) {
                foreach (CardValue cardValue in onlyTypeInfo[CardType.bomb]) {
                    if (cardValue > typeInfo.mainValue) {
                        tipDatas.Add(infos[cardValue].byteDatas);
                    }
                }
            }
        }
        List<List<byte>> GetBombSingleBomb() {
            List<List<byte>> listBomb = new List<List<byte>>();
            if (onlyTypeInfo[CardType.bomb].Count > 0) {
                foreach (CardValue value in onlyTypeInfo[CardType.bomb]) {
                    if (value > typeInfo.mainValue) {
                        listBomb.Add(new List<byte>(infos[value].byteDatas));
                    }
                }
            }
            return listBomb;
        }
        void GetBombSingle() {
            List<List<byte>> listBomb = GetBombSingleBomb();
            if (listBomb.Count > 0 && onlyTypeInfo[CardType.single].Count > 1) {
                foreach (List<byte> listByte in listBomb) {
                    listByte.AddRange(infos[onlyTypeInfo[CardType.single][0]].byteDatas);
                    listByte.AddRange(infos[onlyTypeInfo[CardType.single][1]].byteDatas);
                    tipDatas.Add(listByte);
                }
            }
        }
        void GetBombPair() {
            List<List<byte>> listBomb = GetBombSingleBomb();
            if (listBomb.Count > 0 && onlyTypeInfo[CardType.pair].Count > 1) {
                foreach (List<byte> listByte in listBomb) {
                    listByte.AddRange(infos[onlyTypeInfo[CardType.pair][0]].byteDatas);
                    listByte.AddRange(infos[onlyTypeInfo[CardType.pair][1]].byteDatas);
                    tipDatas.Add(listByte);
                }
            }
        }
        void GetRocketForOther() {
            if (tipDatas.Count > 0) {
                return;
            }
            if (infos.ContainsKey(CardValue.blackJoker) && infos.ContainsKey(CardValue.redJoker)) {
                tipDatas.Add(new List<byte> { infos[CardValue.blackJoker].byteDatas[0], infos[CardValue.redJoker].byteDatas[0] });
            }
        }
        void GetRocket() {
            if (infos.ContainsKey(CardValue.blackJoker) && infos.ContainsKey(CardValue.redJoker)) {
                tipDatas.Add(new List<byte> { infos[CardValue.blackJoker].byteDatas[0], infos[CardValue.redJoker].byteDatas[0] });
            }
        }
        void GetSequence(CardType type) {
            for (CardValue i = ConstData.minCardValue; i < ConstData.maxSequenceValue; i++) {
                if (infos.ContainsKey(i)) {
                    if (Utility.IsType(infos[i], type) && infos[i].CompareTo(typeInfo, type) > 0) {
                        CardValue key = typeInfo.sequenceData[type].Start;
                        do {
                            List<byte> byteDatas = new List<byte>();
                            for (key += 1; key < key + typeInfo.Count(type); key++) {
                                for (byte j = 0; j < Utility.GetSequenceRequireCount(type); j++) {
                                    byteDatas.Add(infos[key].byteDatas[j]);
                                }
                            }
                            tipDatas.Add(byteDatas);
                        } while ((infos[i].sequenceData[type].End - key) >= Utility.GetSequenceRequireLength(type));
                    }
                }
            }
        }

        List<List<byte>> tipDatas = new List<List<byte>>();
        Dictionary<CardType, List<CardValue>> onlyTypeInfo;
        SortedDictionary<CardValue, TypeInfo> infos;
        TypeInfo typeInfo;
        public override string ToString() {
            string str = "\r\n绝对牌型数:\r\n";
            foreach (KeyValuePair<CardType, List<CardValue>> item in onlyTypeInfo) {
                str += "牌型:\t" + item.Key + "\r\n";
                foreach (CardValue value in item.Value) {
                    str += value + ",\t";
                }
                str += "\r\n";
            }
            str += "\r\n复合牌型数据:\r\n";
            foreach (KeyValuePair<CardValue, TypeInfo> item in infos) {
                str += "\r\n" + item.Value.ToString() + "\r\n";
            }
            str += "寻找提示的牌型信息:\r\n" + typeInfo.ToString();
            str += "\r\n提示数据:\r\n";
            for (byte i = 0; i < tipDatas.Count; i++) {
                str += "提示" + i + ":\t";
                foreach (byte byteData in tipDatas[i]) {
                    str += Utility.GetCardValue(byteData) +"(" + byteData + "),\t";
                }
                str += "\r\n";
            }
            return str;
        }
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
                infos[i].sequenceData[type].Start = start;
                infos[i].sequenceData[type].End = end;
            }
        }
    }

}