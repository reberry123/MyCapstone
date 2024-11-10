using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;
using UnityEngine.InputSystem;

public class UIInputField : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField keyword;
    [SerializeField]
    private Button search;
    [SerializeField]
    private GameObject contents;
    Dictionary<string, string> fields = new Dictionary<string, string>
    {
        {"Andromeda", "�ȵ�θ޴��ڸ�"},
        {"Antlia", "���������ڸ�"},
        {"Apus", "�ض����ڸ�"},
        {"Aquarius", "�����ڸ�"},
        {"Aquila", "�������ڸ�"},
        {"Ara", "�����ڸ�"},
        {"Aries", "���ڸ�"},
        {"Auriga", "�������ڸ�"},
        {"Bootes", "���ڸ�"},
        {"Caelum", "����Į�ڸ�"},
        {"Camelopardalis", "�⸰�ڸ�"},
        {"Cancer", "���ڸ�"},
        {"CanesVenatici", "��ɰ��ڸ�"},
        {"CanisMajor", "ū���ڸ�"},
        {"CanisMinor", "�������ڸ�"},
        {"Capricornus", "�����ڸ�"},
        {"Carina", "����ڸ�"},
        {"Cassiopeia", "ī�ÿ����̾��ڸ�"},
        {"Centaurus", "��Ÿ��罺�ڸ�"},
        {"Cepheus", "����콺�ڸ�"},
        {"Cetus", "���ڸ�"},
        {"Chamaeleon", "ī�᷹���ڸ�"},
        {"Circinus", "���۽��ڸ�"},
        {"Columba", "��ѱ��ڸ�"},
        {"ComaBerenices", "�Ӹ����ڸ�"},
        {"CoronaAustralis", "���ʿհ��ڸ�"},
        {"CoronaBorealis", "���ʿհ��ڸ�"},
        {"Corvus", "����ڸ�"},
        {"Crater", "���ڸ�"},
        {"Crux", "�������ڸ�"},
        {"Cygnus", "�����ڸ�"},
        {"Delphinus", "�����ڸ�"},
        {"Dorado", "Ȳ��ġ�ڸ�"},
        {"Draco", "���ڸ�"},
        {"Equuleus", "�������ڸ�"},
        {"Eridanus", "�����ٴ����ڸ�"},
        {"Fornax", "ȭ���ڸ�"},
        {"Gemini", "�ֵ����ڸ�"},
        {"Grus", "�η���ڸ�"},
        {"Hercules", "���Ŭ�����ڸ�"},
        {"Horologium", "�ð��ڸ�"},
        {"Hydra", "�ٴٹ��ڸ�"},
        {"Hydrus", "�����ٴٹ��ڸ�"},
        {"Indus", "�ε���ڸ�"},
        {"Lacerta", "�������ڸ�"},
        {"Leo", "�����ڸ�"},
        {"LeoMinor", "���������ڸ�"},
        {"Lepus", "�䳢�ڸ�"},
        {"Libra", "õĪ�ڸ�"},
        {"Lupus", "�̸��ڸ�"},
        {"Lynx", "����Ҵ��ڸ�"},
        {"Lyra", "�Ź����ڸ�"},
        {"Mensa", "���̺���ڸ�"},
        {"Microscopium", "���̰��ڸ�"},
        {"Monoceros", "�ܻԼ��ڸ�"},
        {"Musca", "�ĸ��ڸ�"},
        {"Norma", "��"},
        {"Octans", "�Ⱥ����ڸ�"},
        {"Ophiuchus", "�������ڸ�"},
        {"Orion", "�������ڸ�"},
        {"Pavo", "�����ڸ�"},
        {"Pegasus", "�䰡�����ڸ�"},
        {"Perseus", "�丣���콺�ڸ�"},
        {"Phoenix", "��Ȳ�ڸ�"},
        {"Pictor", "ȭ���ڸ�"},
        {"Pisces", "������ڸ�"},
        {"PiscisAustrinus", "���ʹ�����ڸ�"},
        {"Puppis", "���ڸ�"},
        {"Pyxis", "��ħ���ڸ�"},
        {"Reticulum", "�׹��ڸ�"},
        {"Sagitta", "ȭ���ڸ�"},
        {"Sagittarius", "�ü��ڸ�"},
        {"Scorpius", "�����ڸ�"},
        {"Sculptor", "�������ڸ�"},
        {"Scutum", "�����ڸ�"},
        {"Serpens", "���ڸ�"},
        {"Sextans", "�������ڸ�"},
        {"Taurus", "Ȳ���ڸ�"},
        {"Telescopium", "�������ڸ�"},
        {"Triangulum", "�ﰢ���ڸ�"},
        {"Triangulum Australe", "���ʻﰢ���ڸ�"},
        {"Tucana", "ū�θ����ڸ�"},
        {"UrsaMajor", "ū���ڸ�"},
        {"UrsaMinor", "�������ڸ�"},
        {"Vela", "���ڸ�"},
        {"Virgo", "ó���ڸ�"},
        {"Volans", "��ġ�ڸ�"},
        {"Vulpecula", "�����ڸ�"},
        {"Sun","�¾�" },
        {"Moon","��" },
        {"Mercury","����"},
        { "Venus","�ݼ�"},
        { "Mars","ȭ��"},
        {"Jupiter","��" },
        { "Saturn","�伺"},
        { "Uranus","õ�ռ�"},
        {"Neptune","�ؿռ�" }

    };

    public void SearchButtonClick()
    {
        string text = keyword.text;
        foreach (KeyValuePair<string,string> item in fields)
        {
            if (item.Value == text)
            {
                text= item.Key;
            }
        }
        StartCoroutine(GetRequest(text));
    }
    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://port-0-capstoneserver-m2qhwewx334fe436.sel4.cloudtype.app/api/stellar/" + url)) 
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.responseCode == 200)
            {
                Debug.Log(webRequest.downloadHandler.text);
                contents.GetComponent<UIScrollView>().MakeItem(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError(webRequest.error);
            }
        }
    }
}