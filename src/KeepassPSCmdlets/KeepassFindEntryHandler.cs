using KeePassLib;
using KeepassPSCmdlets.Extensions;
using System;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace KeepassPSCmdlets
{
    public class KeepassFindEntryHandler : IKPEntryHandler
    {
        private readonly KeepassFindEntryParameter _parameterProvider;

        public KeepassFindEntryHandler(KeepassFindEntryParameter parameterProvider)
        {
            _parameterProvider = parameterProvider ?? throw new ArgumentNullException(nameof(parameterProvider));
        }

        public Func<PwDatabase, PwEntry, bool> EntryHandler => EntryHandlerFunction;

        public bool IncludeRecycleBin { get; set; }

        private bool IsGroupMatch(PwGroup entry)
        {
            if (_parameterProvider.Group == null)
                return true;

            switch (_parameterProvider.GroupFilterMode)
            {
                case FilterMode.Wildcard:
                    return _parameterProvider.Group.Select(group => new WildcardPattern(group)).Any(wp => _parameterProvider.GroupFilterOnPath ? wp.IsMatch(entry.CreateGroupPath()) : wp.IsMatch(entry.Name));

                case FilterMode.RegularExpression:
                    return _parameterProvider.Group.Select(group => new Regex(group)).Any(regex => _parameterProvider.GroupFilterOnPath ? regex.IsMatch(entry.CreateGroupPath()) : regex.IsMatch(entry.Name));
            }
            return false;
        }

        private bool IsTagMatch(PwEntry pwEntry)
        {
            if (_parameterProvider.Tags == null)
                return true;

            switch (_parameterProvider.TagMatchMode)
            {
                case TagMatchMode.WildcardAny:
                    {
                        var tagPatterns = _parameterProvider.Tags.Select(tag => new WildcardPattern(tag));
                        return tagPatterns.Any(wp => pwEntry.Tags.Any(wp.IsMatch));
                    }
                case TagMatchMode.ExactAll:
                    {
                        return pwEntry.Tags.Intersect(_parameterProvider.Tags, StringComparer.CurrentCulture).Count() == _parameterProvider.Tags.Length;
                    }
            }

            return false;
        }

        private bool IsTitleMatch(PwEntry entry)
        {
            if (_parameterProvider.Title == null)
                return true;

            var entryTitle = entry.Strings.Get(PwDefs.TitleField).ReadString();
            switch (_parameterProvider.TitleFilterMode)
            {
                case FilterMode.Wildcard:
                    return _parameterProvider.Title.Select(title => new WildcardPattern(title))
                        .Any(wp => wp.IsMatch(entryTitle));

                case FilterMode.RegularExpression:
                    return _parameterProvider.Title.Select(title => new Regex(title)).Any(regex => regex.IsMatch(entryTitle));
            }
            return false;
        }

        private bool EntryHandlerFunction(PwDatabase db, PwEntry pwEntry)
        {
            if (!IncludeRecycleBin && db.RecycleBinEnabled && db.IsEntryInRecycleBin(pwEntry))
                return false;

            if (IsGroupMatch(pwEntry.ParentGroup))
            {
                return IsTitleMatch(pwEntry) && IsTagMatch(pwEntry);
            }

            return false;
        }
    }
}