using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering.Universal;

public class PlayerTorch : MonoBehaviourPun
{
	public float outerRadius = 5f;
	public float innerRadius = 0.3f;
	public float intensity = 1f;
	public Color color = Color.white;
	public float globalLightIntensity = 0f;

	private Light2D torch;

	void Start()
	{
		if (photonView != null && !photonView.IsMine) return;

		var allLights = FindObjectsOfType<Light2D>();
		for (int i = 0; i < allLights.Length; i++)
		{
			if (allLights[i].lightType == Light2D.LightType.Global)
				allLights[i].intensity = globalLightIntensity;
		}

		var go = new GameObject("TorchLight");
		go.transform.SetParent(transform, false);
		torch = go.AddComponent<Light2D>();
		torch.lightType = Light2D.LightType.Point;
		torch.intensity = intensity;
		torch.color = color;
		torch.pointLightOuterRadius = outerRadius;
		torch.pointLightInnerRadius = innerRadius;
		torch.falloffIntensity = 0.6f;
		torch.shadowsEnabled = false;
	}
}


