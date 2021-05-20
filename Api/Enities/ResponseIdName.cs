using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class ResponseIdName
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ResponseIdName() { }
        public ResponseIdName(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public ResponseIdName(Api.Models.Service service)
        {
            this.Id = service.Id;
            this.Name = service.Name;
        }
        public ResponseIdName(Skill skill)
        {
            this.Id = skill.Id;
            this.Name = skill.Name;
        }  
        public ResponseIdName(Account account)
        {
            this.Id = account.Id;
            this.Name = account.Name;
        }
        public ResponseIdName(Specialty specialty)
        {
            this.Id = specialty.Id;
            this.Name = specialty.Name;
        }
        public ResponseIdName(Level level)
        {
            this.Id = level.Id;
            this.Name = level.Name;
        }
        public ResponseIdName(Role role)
        {
            this.Id = role.Id;
            this.Name = role.Name;
        }
        public ResponseIdName(FormOfWork form)
        {
            this.Id = form.Id;
            this.Name = form.Name;
        }
        public ResponseIdName(Payform pay)
        {
            this.Id = pay.Id;
            this.Name = pay.Name;
        }
        public ResponseIdName(TypeOfWork type)
        {
            this.Id = type.Id;
            this.Name = type.Name;
        }
    }
}
