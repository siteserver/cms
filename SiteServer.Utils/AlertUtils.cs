namespace SiteServer.Utils
{
    public static class AlertUtils
    {
        public static string Error(string title, string text)
        {
            var script = $@"
event && event.preventDefault();
swal({{
  title: '{title}',
  text: '{StringUtils.ReplaceNewline(text, string.Empty)}',
  icon: 'error',
  button: '关 闭',
}});";
            return script;
        }

        public static string Success(string title, string text)
        {
            return Success(title, text, "关 闭", string.Empty);
        }

        public static string Success(string title, string text, string button, string scripts)
        {
            if (!string.IsNullOrEmpty(scripts))
            {
                scripts = $@".then(function (value) {{
  {scripts}
}})";
            }
            var script = $@"
event && event.preventDefault();
swal({{
  title: '{title}',
  text: '{StringUtils.ReplaceNewline(text, string.Empty)}',
  icon: 'success',
  button: '{button}',
}}){scripts};";
            return script;
        }

        public static string Warning(string title, string text, string btnCancel, string btnSubmit, string scripts)
        {
            var script = $@"
event && event.preventDefault();
swal({{
  title: '{title}',
  text: '{StringUtils.ReplaceNewline(text, string.Empty)}',
  icon: 'warning',
  buttons: {{
    cancel: '{btnCancel}',
    catch: '{btnSubmit}'
  }}
}})
.then(function(willDelete){{
  if (willDelete) {{
    {scripts}
  }}
}});";
            return script;
        }

        public static string ConfirmDelete(string title, string text, string url)
        {
            return Confirm(title, text, "确认删除", $"location.href = '{url}';");
        }

        public static string ConfirmRedirect(string title, string text, string btnConfirm, string url)
        {
            return Confirm(title, text, btnConfirm, $"location.href = '{url}';");
        }

        public static string Confirm(string title, string text, string btnConfirm, string scripts)
        {
            var script = $@"
event && event.preventDefault();
swal({{
  title: '{title}',
  text: '{StringUtils.ReplaceNewline(text, string.Empty)}',
  icon: 'warning',
  buttons: {{
cancel: {{
    text: '取 消',
    visible: true,
    className: 'btn'
    }},
    confirm: {{
    text: '{btnConfirm}',
    visible: true,
    className: 'btn btn-danger'
    }}
  }}
}})
.then(function(isConfirm){{
  if (isConfirm) {{
    {scripts}
  }}
}});";
            return script;
        }
    }
}
