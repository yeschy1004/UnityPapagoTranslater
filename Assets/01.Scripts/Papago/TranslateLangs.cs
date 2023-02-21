using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class TranslateLangs : MonoBehaviour
{
    private string _inputStr => _transInputField.text;
    private string _resultStr => _transText.text;

    [Header("----------------UI Field---------------")]
    [SerializeField]
    private InputField _transInputField;
    [SerializeField]
    private Text _transText;
    [SerializeField]
    private Button _translateBtn;
    [SerializeField]
    private RectTransform _scrollContent;

    [Header("-------------Setting Field-------------")]
    private string _json;
    [SerializeField]
    private const string _papagoTransUrl = "https://openapi.naver.com/v1/papago/n2mt";
    private const string _XNaverClientId = "X-Naver-Client-Id";
    private const string _XNaverClientSecret = "X-Naver-Client-Secret";
    private const string _XNaverTransUrl = "source=en&target=ko&text=";
    private const string _XNaverTransForm = "application/x-www-form-urlencoded";

    // QuickStart
    // https://developers.naver.com/docs/papago/papago-nmt-overview.md
    private const string _clientID = "CLIENT-ID";
    private const string _clientSecret = "CLIENT-SECRET";

    public class PapagoResponse
    {
        public string srcLangType; //원본 언어
        public string tarLangType; //결과 언어
        public string translatedText; //번역 텍스트
    }

    public enum PapagoREST
    {
        POST,
        GET,
        DELETE,
        UPDATE
    }

    #region Unity Life Cycle

    private void Start()
    {
        _json = string.Empty;

        _translateBtn?.onClick.AddListener(OnClickTranslater);
    }
    #endregion

    #region Papago API
    private void OnClickTranslater()
    {
        if (string.IsNullOrEmpty(_inputStr))
        {
            Debug.Log("[ERROR] Null Input");
            return;
        }

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_papagoTransUrl);

        request.Headers.Add(_XNaverClientId, _clientID);
        request.Headers.Add(_XNaverClientSecret, _clientSecret);
        request.Method = PapagoREST.POST.ToString();

        byte[] byteDataParams = Encoding.UTF8.GetBytes(_XNaverTransUrl + _inputStr);
        request.ContentType = _XNaverTransForm;
        request.ContentLength = byteDataParams.Length;

        Stream st = request.GetRequestStream();
        st.Write(byteDataParams, 0, byteDataParams.Length);
        st.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stream = response.GetResponseStream();
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);

        Debug.Log("Response State: " + response.StatusDescription);

        _json = reader.ReadToEnd();
        stream.Close();
        response.Close();
        reader.Close();

        SetTransText();
    }

    private void SetTransText()
    {
        JObject jObject = JObject.Parse(_json);
        Debug.Log("JSON: " + _json);
        _transText.text = jObject["message"]["result"]["translatedText"].ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollContent);
    }
    #endregion
}
