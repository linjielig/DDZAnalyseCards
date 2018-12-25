using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnalyseCards;
public class TestTipLogic : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.T)) {
            //byte[] byteDatas = Utility.GenerateDatas();
            Analyse.Instance.GetTipDatas(
            new byte[] { 0x7, 0x7, 0x7, 0x7, 0x8, 0x8, 0x8, 0x8 }, 
            new byte[] { 0x3, 0x3, 0x3, 0x3 }, false);
        }
    }
    public void TestSingle() {
        byte[] byteDatas = Utility.GetSinglePairThreeBombDatas(CardType.single);
        for (byte i = 0; i < byteDatas.Length; i++) {
            Analyse.Instance.IsWithRule(new byte[] { byteDatas[i] });
            Debug.Log(Analyse.Instance.analyseTypeInfo);

        }
    }
    public void TestPair() {
        byte[] byteDatas = Utility.GetSinglePairThreeBombDatas(CardType.pair);
        for (byte i = 0; i < byteDatas.Length / 2; i++) {
            Analyse.Instance.IsWithRule(new byte[] { byteDatas[i], byteDatas[i + 13]});
            Debug.Log(Analyse.Instance.analyseTypeInfo);

        }
    }
    public void TestThree() {
        byte[] byteDatas = Utility.GetSinglePairThreeBombDatas(CardType.three);
        for (byte i = 0; i < byteDatas.Length / 3; i++) {
            Analyse.Instance.IsWithRule(new byte[] { byteDatas[i], byteDatas[i + 13], byteDatas[i + 13 + 13] });
            Debug.Log(Analyse.Instance.analyseTypeInfo);
        }
    }
    public void TestBomb() {
        byte[] byteDatas = Utility.GetSinglePairThreeBombDatas(CardType.bomb);
        for (byte i = 0; i < byteDatas.Length / 4; i++) {
            Analyse.Instance.IsWithRule(new byte[] { byteDatas[i], byteDatas[i + 13], byteDatas[i + 13 * 2], byteDatas[i + 13 * 3] });
            Debug.Log(Analyse.Instance.analyseTypeInfo);
        }
    }
    public void TestThreeSingle() {
        byte[] byteDatas = new byte[] { 0x3, 0x13, 0x23, 0x1};
        Analyse.Instance.IsWithRule(byteDatas);
        Debug.Log(Analyse.Instance.analyseTypeInfo);
    }
    public void TestThreePair() {
        byte[] byteDatas = new byte[] { 0x3, 0x13, 0x23, 0x1, 0x11 };
        Analyse.Instance.IsWithRule(byteDatas);
        Debug.Log(Analyse.Instance.analyseTypeInfo);
    }
    public void TestBombSingle() {
        byte[] byteDatas = new byte[] { 0x3, 0x13, 0x23, 0x33, 0x1, 0x2 };
        Analyse.Instance.IsWithRule(byteDatas);
        Debug.Log(Analyse.Instance.analyseTypeInfo);
    }
    public void TestBombPair() {
        byte[] byteDatas = new byte[] { 0x3, 0x13, 0x23, 0x33, 0x1, 0x11, 0x2, 0x12 };
        Analyse.Instance.IsWithRule(byteDatas);
        Debug.Log(Analyse.Instance.analyseTypeInfo);
    }
    public void TestSequence() {
        byte[] byteDatas = new byte[] { 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd };
        Analyse.Instance.IsWithRule(byteDatas);
        Debug.Log(Analyse.Instance.analyseTypeInfo);
    }
    public void TestSequencePair() {
        byte[] byteDatas = new byte[] { 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd,
        0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd  };
        Analyse.Instance.IsWithRule(byteDatas);
        Debug.Log(Analyse.Instance.analyseTypeInfo);
    }
    public void TestSequenceThree() {
        byte[] byteDatas = new byte[] { 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd,
        0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd,
        0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd  };
        Analyse.Instance.IsWithRule(byteDatas);
        Debug.Log(Analyse.Instance.analyseTypeInfo);
    }
    public void TestSequenceThreeSingle() {
        byte[] byteDatas = new byte[] { 0x3, 0x4, 0x5, 0xb, 0xc, 0xd,
        0x3, 0x4, 0x5,
        0x3, 0x4, 0x5  };
        Analyse.Instance.IsWithRule(byteDatas);
        Debug.Log(Analyse.Instance.analyseTypeInfo);
    }
    public void TestSequenceThreePair() {
        byte[] byteDatas = new byte[] { 0x3, 0x4, 0x5, 0xb, 0xc, 0xd,
        0x3, 0x4, 0x5, 0xb, 0xc, 0xd,
        0x3, 0x4, 0x5 };
        Analyse.Instance.IsWithRule(byteDatas);
        Debug.Log(Analyse.Instance.analyseTypeInfo);
    }
}
