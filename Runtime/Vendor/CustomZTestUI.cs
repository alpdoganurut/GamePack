using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace Glitchers.UserInterface
{
	/// <summary>
	/// https://gist.github.com/glitchersgames/5a9b5f256c1f160d92399749a4776d8d
	/// </summary>
	public class CustomZTestUI : MonoBehaviour, IMaterialModifier
	{
		#region Serialized
		[Tooltip("LessEqual is 'normal'. Always is overlay. Never is hide.")]
		public CompareFunction comparison = CompareFunction.LessEqual;
		#endregion

		#region Variables
		private Graphic m_Graphic;
		private Material m_RenderMaterial;
		#endregion

		#region Properties
		private const string _propertyKey = "unity_GUIZTestMode";
		private static int? _propertyID;
		private static int PropertyID
		{
			get
			{
				if(_propertyID.HasValue==false)
				{
					_propertyID = Shader.PropertyToID(_propertyKey);
				}
				return _propertyID.Value;
			}
		}
		#endregion

		#region Lifecycle
		private void Awake()
		{
			m_Graphic = GetComponent<Graphic>();
		}

		private void OnEnable()
		{
			SetDirty();
		}

		private void OnDisable()
		{
			SetDirty();
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if(m_Graphic==null)
			{
				m_Graphic = GetComponent<Graphic>();
			}

			SetDirty();
		}
#endif
		#endregion

		#region Methods
		private void SetDirty()
		{
			if(m_Graphic!=null)
			{
				m_Graphic.SetMaterialDirty();
			}
		}

		// Most common use-case
		public void SetOverlay( bool overlay )
		{
			comparison = (overlay ? CompareFunction.Always : CompareFunction.LessEqual);
			SetDirty();
		}
		#endregion

		#region IMaterialModifier
		Material IMaterialModifier.GetModifiedMaterial(Material baseMaterial)
		{
#if UNITY_EDITOR
			if( Application.isPlaying == false )
			{
				return baseMaterial;
			}
#endif

			if(m_RenderMaterial==null)
			{
				m_RenderMaterial = new Material(baseMaterial)
				{
					name = string.Format("{0} CustomZTestUI", baseMaterial.name),
					hideFlags = HideFlags.HideAndDontSave
				};
			}
			
			m_RenderMaterial.SetInt(PropertyID, (int)comparison);
			
			return m_RenderMaterial;
		}
		#endregion
	}
}