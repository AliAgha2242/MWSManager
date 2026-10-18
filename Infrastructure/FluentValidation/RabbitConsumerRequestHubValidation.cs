using FluentValidation;
using Model.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.FluentValidation
{
    public class RabbitConsumerRequestHubValidation : AbstractValidator<RabbitConsumerRequestHub>
    {
        public RabbitConsumerRequestHubValidation()
        {
            RuleFor(x => x.ProcessorName)
            .NotEmpty().WithMessage("نام تابع نمیتواند خالی باشد ");
            RuleFor(c => c.RequestName).NotEmpty().Must(p => !string.IsNullOrEmpty(p)).WithMessage("نام نباید خالی باشد ");
        }
    }
    public class RabbitTasksValidation : AbstractValidator<RabbitTasks>
    {
        public RabbitTasksValidation()
        {
           
			//RuleFor(c => c.IntervalMinut).Must(p => p < 60).WithMessage("دقیقه نمیتواند بیشتر از 59 باشد");
            RuleFor(c => c.TaskName).NotEmpty().Must(p => !string.IsNullOrEmpty(p)).WithMessage("نام نباید خالی باشد ");
            RuleFor(x => x.IntervalTotalMinute).Must((x,p) => p >= 1 || x.IntervalHour >= 1).WithMessage("زمان دوره کمتر از حداقل مجاز میباشد");
			RuleFor(x => x.RabbitConsumerRequestHubId).Must(p => p > 0 || p != null).WithMessage("درخواست یا پیام آماده انتخاب نشده است");

		}
	}
}
