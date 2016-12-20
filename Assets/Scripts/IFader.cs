using System.Collections;

public interface IFader
{
    IEnumerator FadeIn();
    IEnumerator FadeOut();
}