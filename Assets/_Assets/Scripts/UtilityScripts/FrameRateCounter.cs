using UnityEngine;
using TMPro;

public class FrameRateCounter : MonoBehaviour 
{
    public enum DisplayMode { FPS, MS }

	[SerializeField] private DisplayMode _displayMode = DisplayMode.FPS;
	[SerializeField] private TextMeshProUGUI _display;
    [SerializeField, Range(.1f, 2f)] private float _sampleDuration = 1f;

    private int _frames;
    private float _duration;
    private float _bestDuration = float.MaxValue;
    private float _worstDuration;

    private void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;
        _frames++;
        _duration += frameDuration;

        if (frameDuration < _bestDuration) {
			_bestDuration = frameDuration;
		}
		if (frameDuration > _worstDuration) {
			_worstDuration = frameDuration;
		}

        if(_duration >= _sampleDuration)
        {
            if(_displayMode == DisplayMode.FPS)
            {
                _display.SetText(
                    "Best FPS \n {0:0} \n Average FPS \n {1:0} \n Worst FPS \n {2:0}",
                    1f / _bestDuration,
                    _frames / _duration,
                    1f / _worstDuration
                );
            }
            else
            {
                _display.SetText(
					"Best MS \n {0:1} \n Average MS \n {1:1} \n Worst MS \n {2:1}",
					1000f * _bestDuration,
					1000f * _duration / _frames,
					1000f * _worstDuration
				);
            }
			_frames = 0;
			_duration = 0f;
			_bestDuration = float.MaxValue;
			_worstDuration = 0f;
        }
    }
}