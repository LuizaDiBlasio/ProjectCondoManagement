using ClassLibrary;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ProjectCondoManagement.Helpers
{
    public class SmsHelper : ISmsHelper
    {
        private readonly IConfiguration _configuration;

        public SmsHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Response<object>> SendSmsAsync(string phoneNumber, string message)
        {
            //  configuração
            var accountSid = _configuration["TwilioSettings:AccountSid"];
            var authToken = _configuration["TwilioSettings:AuthToken"];
            var twilioPhoneNumber = _configuration["TwilioSettings:TwilioPhoneNumber"];

            // Inicializar  Twilio
            TwilioClient.Init(accountSid, authToken);

            try
            {
                //Criar e enviar a mensagem
                var messageResource = await MessageResource.CreateAsync(
                    to: new PhoneNumber(phoneNumber),
                    from: new PhoneNumber(twilioPhoneNumber),
                    body: message,
                    riskCheck: MessageResource.RiskCheckEnum.Disable //para poder testar sem block
                );

                //verificar status da mensagem
                if (messageResource.Status == MessageResource.StatusEnum.Queued || messageResource.Status == MessageResource.StatusEnum.Sending)
                {
                    // A mensagem está a caminho.
                    return new Response<object>
                    {
                        IsSuccess = true,
                        Message = $"SMS enviado e em processamento. Status: {messageResource.Status}."
                    };
                }
                else
                {
                    //mensagem falhou
                    return new Response<object>
                    {
                        IsSuccess = false,
                        Message = $"Falha no envio inicial do SMS. Status: {messageResource.Status}."
                    };
                }

            }
            catch (Exception ex)
            {
                // tratar exceções
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
