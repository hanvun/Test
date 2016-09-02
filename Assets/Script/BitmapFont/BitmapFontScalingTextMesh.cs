using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BitmapFontScalingTextMesh : MonoBehaviour
{
	[SerializeField] int textSize;
	float bitmapFontOriginScale;
	TextMesh tm;

	void Awake(){
		tm = (TextMesh)gameObject.GetComponent (typeof(TextMesh));
	}

    public void OnValidate()
	{
		if(tm == null )
			tm = (TextMesh)gameObject.GetComponent (typeof(TextMesh));
		 
		if( tm == null || tm.font == null || tm.font.dynamic == true )
		{
			Debug.LogWarning("Bitmap font is not found.");
			return;
		}

		GetFontSize();
		UpdateScale();
	}

	void GetFontSize()
	{
		textSize = tm.fontSize;

		bitmapFontOriginScale = tm.font.lineHeight;
	}

	void UpdateScale()
	{
		float scale;
		
		if( textSize <= 0 || bitmapFontOriginScale <= 0 )
			scale = 1f;
		else
			scale = (float)textSize / bitmapFontOriginScale;
		
		transform.localScale = new Vector3(scale, scale, scale);
	}
}