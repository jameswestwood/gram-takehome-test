using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class TweenHelper
{
	public static void SafeKill(this Tween tween)
	{
		if (tween != null && tween.IsPlaying())
		{
			tween.Kill();
		}
	}

	public static void SafeKill(this Sequence sequence)
	{
		if (sequence != null && sequence.IsPlaying())
		{
			sequence.Kill();
		}
	}
}
