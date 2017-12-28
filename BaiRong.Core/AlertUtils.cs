namespace BaiRong.Core
{
    public class AlertUtils
    {
        private AlertUtils()
        {
        }

        public static string Error(string title, string text)
        {
            var script = $@"swal({{
  title: '{title}',
  text: '{StringUtils.ReplaceNewline(text, string.Empty)}',
  icon: 'error',
  button: '关 闭',
}});";
            return script;
        }

        public static string Success(string title, string text)
        {
            return Success(title, text, "关 闭", null);
        }

        public static string Success(string title, string text, string button, string scripts)
        {
            if (!string.IsNullOrEmpty(scripts))
            {
                scripts = $@".then(function (value) {{
  {scripts}
}})";
            }
            var script = $@"swal({{
  title: '{title}',
  text: '{StringUtils.ReplaceNewline(text, string.Empty)}',
  icon: 'success',
  button: '{button}',
}}){scripts};";
            return script;
        }

        public static string Warning(string title, string text, string btnCancel, string btnSubmit, string scripts)
        {
            var script = $@"swal({{
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
    }
}
