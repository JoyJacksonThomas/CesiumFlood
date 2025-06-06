﻿using System;
using UnityEngine;

// Link System Tutorial 1: Showing a tooltip by clicking on text in a Text Mesh Pro textbox
// Video tutorial: https://youtu.be/N6vYyCahLr8

// Hi! I'm Christina, this is my implementation of how to make text in a Text Mesh Pro textbox clickable. 
// This system uses three scripts:
// TooltipHandler - Controls the tooltip and I place it on my Canvas gameobject which contains the tooltip to control
// LinkHandlerForTMPText - Sits on the same gameobject as your Text Mesh Pro component, but could live anywhere else, too. It checks if you have clicked on a link in your textbox.
// TooltipInfos - a simple struct which contains relevant infos for the link. In this implementation, it uses an image to display, in the implementation on showing a tooltip while hovering, it contains different contents.
// Please watch the video if you have questions about how to set this up :)
// Hope you'll enjoy my system!
// - Christina

namespace ChristinaCreatesGames
{
    [Serializable]
    public struct TooltipInfos
    {
        public string Keyword;
        public string Description;
        public Sprite Image;
        
    }
}