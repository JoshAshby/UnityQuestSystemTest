using System;

namespace Ashode
{
    public class EventSystem
    {
        public Action Repaint;
        public void OnRepaint() { if (Repaint != null) Repaint(); }
    }
}