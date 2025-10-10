using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Toolkit.UI;

namespace BadMC_Launcher.Helpers;

public static class UIHelper {
    /// <summary>
    /// Finds the corresponding element in the given visual tree via x:Name.
    /// </summary>
    /// <param name="element">Provide the root element to identify where to look.</param>
    /// <param name="name">The x:Name attribute of the element to be looked up.</param>
    /// <returns>Returns the element instance corresponding to the given name.</returns>
    public static UIElement? FindElementByName(UIElement element, string name) {
        if (string.IsNullOrEmpty(name)) {
            return null;
        }
        
        // Get element of name.
        return FindElementRecursive(element, name);
    }

    private static UIElement? FindElementRecursive(DependencyObject parent, string name) {
        // Gets all child elements of the root element.
        int childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childCount; i++) {
            var child = VisualTreeHelper.GetChild(parent, i) as UIElement;
            if (child == null) {
                continue;
            }

            // Returns the target element if the names match.
            if (child is FrameworkElement frameworkChild && frameworkChild.Name == name) {
                return child;
            }

            // Recursively find the subtree of a child element.
            var result = FindElementRecursive(child, name);
            if (result != null) {
                return result;
            }
        }

        // ContentControl
        if (parent is ContentControl contentControl) {
            var content = contentControl.Content as UIElement;
            if (content != null) {
                var result = FindElementRecursive(content, name);
                if (result != null) {
                    return result;
                }
            }
        }

        // Panel
        if (parent is Panel panel) {
            foreach (var child in panel.Children) {
                var result = FindElementRecursive(child, name);
                if (result != null) {
                    return result;
                }
            }
        }

        return null;
    }
}
