using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.StringConstants;

public static class ResponseMessages
{
    public const string EmailExists = "Така пошта вже використовується";
    public const string LoginExists = "Такий логін вже використовується";
    public const string NonActivatedUser = "Користувач не активований";
    public const string IncorrectUser = "Такого користувача немає або пароль чи нікнейм неправильні";
    public const string InvalidShortLoginToken = "Такого користувача немає або термін дії токену сплив";
    public const string InvalidActivationToken = "Посилання не дійсне або застаріле. Отримайте новий код";
    public const string InvalidUploadToken = "Здається, це не Ви власник подкасту";
    public const string SuccessfulActivation = "Підтвердження успішне! Можете авторизуватися";
    public const string NeedToLogin = "Потрібно авторизуватися";
    public const string ImageAccepted = "Зображення додано";
    public const string FileAccepted = "Файл прийняв";
    public const string NoImage = "Зображення не знайдено";
    public const string ReadyToFileAccept = "Готовий приймати";
    
}
