
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Nomlas.UdonPortal
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ComponentChecker : UdonSharpBehaviour
    {
        [Header("UdonPortal")][SerializeField] private Transform udonPortal;
        [Header("ポータルのワールドやインスタンスIDなどを表示するTMP")][SerializeField] private TextMeshProUGUI roomData;
        [Header("生の階層構造を出力するTMP")][SerializeField] private TMP_InputField rawHierarchy;
        [Header("ポータルをイメージとして表示する先")][SerializeField] private Transform portalParent;
        [Header("ポータルセレクターの親")][SerializeField] private Transform selectorParent;
        [Header("Joinヘルパー")][SerializeField] private GameObject join;
        [Header("ポータルセレクターのメッシュと置き換えるBoxメッシュ")][SerializeField] private Mesh boxMesh;
        [Header("ポータル情報を更新する間隔")][SerializeField] private float checkInterval = 5f;
        private GameObject portal;
        private Transform selector;
        private bool follow;

        private void Start()
        {
            Check();
        }

        /*
        private void Update()
        {
            if (follow && (selector != null))
            {
                selector.SetPositionAndRotation(selectorParent.position, selectorParent.rotation);
            }
        }
        */

        private void UpdatePortalData()
        {
            var canvasRoot = udonPortal.Find("VRCPortalMarker(Clone)/PortalInternal(Clone)/Canvas");
            if (canvasRoot == null) return;
            Transform WorldTextT = canvasRoot.Find("WorldText");
            Transform OwnerTextT = canvasRoot.Find("OwnerText");
            Transform AccessTextT = canvasRoot.Find("AccessText");
            Transform GroupTextT = canvasRoot.Find("GroupText");
            Transform AgeGateTextT = canvasRoot.Find("AgeGateText");
            Transform PlayerCountT = canvasRoot.Find("PlayerCount");
            Transform TimerT = canvasRoot.Find("Timer");
            TextMeshProUGUI WorldTextC = (WorldTextT != null) ? WorldTextT.GetComponent<TextMeshProUGUI>() : null;
            TextMeshProUGUI OwnerTextC = (OwnerTextT != null) ? OwnerTextT.GetComponent<TextMeshProUGUI>() : null;
            TextMeshProUGUI AccessTextC = (AccessTextT != null) ? AccessTextT.GetComponent<TextMeshProUGUI>() : null;
            TextMeshProUGUI GroupTextC = (GroupTextT != null) ? GroupTextT.GetComponent<TextMeshProUGUI>() : null;
            TextMeshProUGUI AgeGateTextC = (AgeGateTextT != null) ? AgeGateTextT.GetComponent<TextMeshProUGUI>() : null;
            TextMeshProUGUI PlayerCountC = (PlayerCountT != null) ? PlayerCountT.GetComponent<TextMeshProUGUI>() : null;
            TextMeshProUGUI TimerC = (TimerT != null) ? TimerT.GetComponent<TextMeshProUGUI>() : null;
            string WorldText = (WorldTextC != null) ? WorldTextC.text : null;
            string OwnerText = (OwnerTextC != null) ? OwnerTextC.text : null;
            string AccessText = (AccessTextC != null) ? AccessTextC.text : null;
            string GroupText = (GroupTextC != null) ? GroupTextC.text : null;
            string AgeGateText = (AgeGateTextC != null) ? AgeGateTextC.text : null;
            string PlayerCount = (PlayerCountC != null) ? PlayerCountC.text : null;
            string Timer = (TimerC != null) ? TimerC.text : null;
            //Debug.Log($"WorldText: {WorldText}, OwnerText: {OwnerText}, AccessText: {AccessText}, GroupText: {GroupText}, AgeGateText: {AgeGateText}, PlayerCount: {PlayerCount}, Timer: {Timer}");
            roomData.text = $"World: {WorldText}\nOwner: {OwnerText}\nAccess: {AccessText}\nGroup: {GroupText}\nAgeGate: {AgeGateText}\nPlayerCount: {PlayerCount}\nTimer: {Timer}";

            if (portal != null)
            {
                GameObject.Destroy(portal);
            }
            var portalPlane = udonPortal.Find("VRCPortalMarker(Clone)/PortalInternal(Clone)/PortalGraphics/Base Graphics/Portal Plane");
            if (portalPlane != null)
            {
                portal = GameObject.Instantiate(portalPlane.gameObject, portalParent);
            }
        }

        public void SelectorHack()
        {
            selector = udonPortal.Find("VRCPortalMarker(Clone)/PortalInternal(Clone)/PortalSelector");
            if (selector != null)
            {
                selector.SetPositionAndRotation(selectorParent.position, selectorParent.rotation);
                selector.localScale = selectorParent.localScale;
                selector.GetComponent<MeshFilter>().mesh = boxMesh;
                selector.GetComponent<BoxCollider>().size = Vector3.one;
                selector.GetComponent<BoxCollider>().center = Vector3.zero;
                selector.gameObject.SetActive(true);
                join.SetActive(true);
                if (Utilities.IsValid(portal)) portal.SetActive(false);
                follow = true;
            }
        }

        public void JoinHide()
        {
            if (Utilities.IsValid(selector)) selector.gameObject.SetActive(false);
            if (Utilities.IsValid(portal)) portal.SetActive(true);
            join.SetActive(false);
            follow = false;
        }

        public void Check()
        {
            UpdatePortalData();
            SendCustomEventDelayedSeconds(nameof(Check), checkInterval);
        }

        public void Write()
        {
            StartPrint();
        }

        void StartPrint()
        {
            rawHierarchy.text = "";
            PrintComponentTree(udonPortal.gameObject, 0);
        }

        [RecursiveMethod]
        void PrintComponentTree(GameObject obj, int indentLevel)
        {
            string s = "";
            string indent = new string(' ', indentLevel * 2);

            rawHierarchy.text += s + indent + "- " + (obj.activeSelf ? "○" : "×") + " " + obj.name + ":\n";

            // GameObjectにアタッチされているコンポーネントをすべて取得し表示
            Component[] components = obj.GetComponents<Component>();
            foreach (Component comp in components)
            {
                if (comp == null)
                {
                    rawHierarchy.text += s + indent + "  - " + "Null Component" + "\n";
                    continue;
                }
                var t = "";
                var type = comp.GetType();
                if (type == typeof(BoxCollider))
                {
                    BoxCollider c = (BoxCollider)comp;
                    t = (c.enabled ? "○" : "×") + " ";
                }
                if (type == typeof(MeshRenderer))
                {
                    MeshRenderer c = (MeshRenderer)comp;
                    t = (c.enabled ? "○" : "×") + " ";
                }
                if (type.Name == "ÎÏÎÍÎÍÏÎÌÏÏÎÍÍÍÎÏÍÏÎÌÌÌ")
                {
                    TextMeshProUGUI c = (TextMeshProUGUI)comp;
                    rawHierarchy.text += s + indent + "  - " + t + "TextMeshProUGUI" + ":\n";
                    rawHierarchy.text += s + indent + "    - text: \"" + c.text + "\"\n";
                }
                else
                {
                    rawHierarchy.text += s + indent + "  - " + t + type.Name + "\n";
                }
            }

            if (obj.transform == null)
            {
                rawHierarchy.text += "Transform error!\n";
                return;
            }
            // 子オブジェクトに対して再帰的に処理
            foreach (Transform child in obj.transform)
            {
                if (child == null)
                {
                    rawHierarchy.text += "child transform error!\n";
                    continue;
                }
                var g = child.gameObject;
                if (g == null)
                {
                    rawHierarchy.text += "child gameObject error!\n";
                    continue;
                }
                PrintComponentTree(g, indentLevel + 1);
            }
        }
    }
}