using Character.Compositor;
using Reactivity;
using System;
using System.Linq;
using UnityEngine;

namespace Character.Creator.UI
{
	public interface IColorCopyPasting
	{
		void Copy();
		void Paste();
		event Action Copied;
		event Action Pasted;
		event Action PasteFailedInvalidFormat;
	}
	public class ColorCopyPasting : MonoBehaviour, IColorCopyPasting
	{
		private IInputRestrictor _inputRestrictor;
		private ICustomizationSelectedDataRepository _dataRepository;
		private ICharacterCreatorUndoManager _undoManager;
		private IColorActiveSelection _activeSelection;

		public event Action Copied = delegate { };
		public event Action Pasted = delegate { };
		public event Action PasteFailedInvalidFormat = delegate { };

		void Awake()
		{
			_inputRestrictor = Singletons.GetSingleton<IInputRestrictor>();
			_dataRepository = GetComponentInParent<ICustomizationSelectedDataRepository>();
			_undoManager = this.GetComponentInParent<ICharacterCreatorUndoManager>();
			_activeSelection = this.GetComponent<IColorActiveSelection>();
		}
		void Update()
		{
			if (!_inputRestrictor.InputAllowed) return; // Input not allowed

			if (!Input.GetKey(KeyCode.LeftControl)) return;
			if (Input.GetKeyDown(KeyCode.C))
			{
				Copy();
			}
			if (Input.GetKeyDown(KeyCode.V))
			{
				Paste();
			}

		}
		public void Copy()
		{
			var id = _activeSelection.FirstSelected;
			if (!id) return;

			var baseHex = GetHexString(_dataRepository.GetColorizeValues(id).Base);
			var shadeHex = GetHexString(_dataRepository.GetColorizeValues(id).Shade);
			GUIUtility.systemCopyBuffer = $"{baseHex} {shadeHex}";

			Copied();
		}

		static string GetHexString(IColorizeValuesPart part)
		{
			return "#" + ColorUtility.ToHtmlStringRGB(part.GetColor());
		}

		public void Paste()
		{
			var (baseColor, shadeColor) = GetColorsFromClipboard();
			if (baseColor == null)
			{
				PasteFailedInvalidFormat();
				return;
			}

			_undoManager.RecordState("Pasted color");

			var ids = _activeSelection.AllSelected.ToList();
			using var suspender = new ReactivityNotificationSuspender();
			foreach (var id in ids)
			{
				var existingColor = _dataRepository.GetColorizeValues(id);

				var newColorizeValues = shadeColor == null
					? new ColorizeValuesFromRealColor(baseColor.Value, existingColor)
					: new ColorizeValuesFromRealColor(baseColor.Value, shadeColor.Value);
				_dataRepository.SetColorizeValues(id, newColorizeValues);
			}
			Pasted();
		}

		static (Color?, Color?) GetColorsFromClipboard()
		{
			string clipboardText = GUIUtility.systemCopyBuffer;

			if (string.IsNullOrWhiteSpace(clipboardText)) return (null, null);

			var colorParts = clipboardText.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

			if (colorParts.Length == 1)
			{
				// Single color format
				var color = ParseSingleColor(colorParts[0]);
				return (color, null);
			}
			else if (colorParts.Length == 2)
			{
				// Two color format
				var colorBase = ParseSingleColor(colorParts[0]);
				var colorShade = ParseSingleColor(colorParts[1]);
				return (colorBase, colorShade);
			}

			return (null, null);
		}

		static Color? ParseSingleColor(string colorString)
		{
			if (string.IsNullOrWhiteSpace(colorString)) return null;

			string normalizedColor = colorString.Trim();
			if (!normalizedColor.StartsWith("#"))
				normalizedColor = "#" + normalizedColor;

			return ColorUtility.TryParseHtmlString(normalizedColor, out Color color) ? color : null;
		}
	}
}