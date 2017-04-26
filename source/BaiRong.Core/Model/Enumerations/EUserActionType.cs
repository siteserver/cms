using System;

namespace BaiRong.Core.Model.Enumerations
{
    public enum EUserActionType
    {
        Login,              //登录成功
        LoginFailed,              //登录失败
        WritingAdd,         //新增稿件
        WritingEdit,        //编辑稿件
        WritingDelete,      //删除稿件
        UpdateEmail,        //修改邮箱
        UpdateMobile,       //修改手机
        UpdatePassword,     //修改密码
    }

    public class EUserActionTypeUtils
    {
        public static string GetValue(EUserActionType type)
        {
            if (type == EUserActionType.Login)
            {
                return "Login";
            }
            else if (type == EUserActionType.LoginFailed)
            {
                return "LoginFailed";
            }
            else if (type == EUserActionType.WritingAdd)
            {
                return "WritingAdd";
            }
            else if (type == EUserActionType.WritingEdit)
            {
                return "WritingEdit";
            }
            else if (type == EUserActionType.WritingDelete)
            {
                return "WritingDelete";
            }
            else if (type == EUserActionType.UpdateEmail)
            {
                return "UpdateEmail";
            }
            else if (type == EUserActionType.UpdateMobile)
            {
                return "UpdateMobile";
            }
            else if (type == EUserActionType.UpdatePassword)
            {
                return "UpdatePassword";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(EUserActionType type)
        {
            if (type == EUserActionType.Login)
            {
                return "用户登录成功";
            }
            else if (type == EUserActionType.LoginFailed)
            {
                return "用户登录失败";
            }
            else if (type == EUserActionType.WritingAdd)
            {
                return "新增稿件";
            }
            else if (type == EUserActionType.WritingEdit)
            {
                return "编辑稿件";
            }
            else if (type == EUserActionType.WritingDelete)
            {
                return "删除稿件";
            }
            else if (type == EUserActionType.UpdateEmail)
            {
                return "修改邮箱";
            }
            else if (type == EUserActionType.UpdateMobile)
            {
                return "修改手机";
            }
            else if (type == EUserActionType.UpdatePassword)
            {
                return "修改密码";
            }
            else
            {
                throw new Exception();
            }
        }

        public static EUserActionType GetEnumType(string typeStr)
        {
            if (Equals(typeStr, EUserActionType.Login))
                return EUserActionType.Login;
            else if (Equals(typeStr, EUserActionType.LoginFailed))
                return EUserActionType.LoginFailed;
            else if (Equals(typeStr, EUserActionType.WritingAdd))
                return EUserActionType.WritingAdd;
            else if (Equals(typeStr, EUserActionType.WritingEdit))
                return EUserActionType.WritingEdit;
            else if (Equals(typeStr, EUserActionType.WritingDelete))
                return EUserActionType.WritingDelete;
            else if (Equals(typeStr, EUserActionType.UpdateEmail))
                return EUserActionType.UpdateEmail;
            else if (Equals(typeStr, EUserActionType.UpdateMobile))
                return EUserActionType.UpdateMobile;
            else if (Equals(typeStr, EUserActionType.UpdatePassword))
                return EUserActionType.UpdatePassword;
            else
                throw new Exception();
        }

        public static bool Equals(string typeStr, EUserActionType type)
        {
            if (string.IsNullOrEmpty(typeStr))
                return false;
            if (string.Equals(typeStr.ToLower(), GetValue(type).ToLower()))
                return true;
            return false;
        }

        public static bool Equals(EUserActionType type, string typeStr)
        {
            return Equals(typeStr, type);
        }
    }
}
