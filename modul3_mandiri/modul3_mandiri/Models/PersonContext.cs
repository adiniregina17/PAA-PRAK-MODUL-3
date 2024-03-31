using Npgsql;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using modul3_mandiri.Helpers;
using modul3_mandiri.Models;

namespace modul3_mandiri.Models
{
    public class PersonContext
    {
        private string __constr;
        private string __ErrorMsg;

        public PersonContext(string pConstr)
        {
            __constr = pConstr;
        }
        public List<Admin> ListPerson()
        {
            List<Admin> list1 = new List<Admin>();
            string query = string.Format(@"SELECT id_person, nama, alamat, email FROM users.person;");
            SqlDBHelper db = new SqlDBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list1.Add(new Admin()
                    {
                        id_person = int.Parse(reader["id_person"].ToString()),
                        nama = reader["nama"].ToString(),
                        alamat = reader["alamat"].ToString(),
                        email = reader["email"].ToString()
                    });
                }
                cmd.Dispose();
                db.closeConnection();
            }
            catch (Exception ex)
            {
                __ErrorMsg = ex.Message;
            }
            return list1;

        }
        public void AddPerson(Admin person)
        {
            string query = string.Format(@"INSERT INTO person (nama, alamat, email) VALUES ('{0}', '{1}', '{2}');",
                person.nama, person.alamat, person.email);
            SqlDBHelper db = new SqlDBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                db.closeConnection();
            }
            catch (Exception ex)
            {
                __ErrorMsg = ex.Message;
            }
        }

        public void UpdatePerson(Admin person)
        {
            string query = string.Format(@"UPDATE users.person SET nama = '{0}', alamat = '{1}', email = '{2}' WHERE id_person = {3};",
                person.nama, person.alamat, person.email, person.id_person);
            SqlDBHelper db = new SqlDBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                db.closeConnection();
            }
            catch (Exception ex)
            {
                __ErrorMsg = ex.Message;
            }
        }

        public void DeletePerson(int id)
        {
            string query = string.Format(@"DELETE FROM users.person WHERE id_person = {0};", id);
            SqlDBHelper db = new SqlDBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                db.closeConnection();
            }
            catch (Exception ex)
            {
                __ErrorMsg = ex.Message;
            }
        }

        public bool IsValidUser(string p_username, string p_password)
        {
            string query = string.Format(@"SELECT COUNT(*)
            FROM users.person ps INNER JOIN users.peran_person pp ON ps.id_person=pp.id_person
            INNER JOIN users.peran p ON pp.id_peran=p.id_peran 
            where ps.username='{0}' and ps.password='{1}' and pp.id_peran=1; ", p_username, p_password);
            SqlDBHelper db = new SqlDBHelper(this.__constr);
            try
            {
                NpgsqlCommand cmd = db.GetNpgsqlCommand(query);
                int count = int.Parse(cmd.ExecuteScalar().ToString());

                cmd.Dispose();
                db.closeConnection();

                return count > 0;
            }
            catch (Exception ex)
            {
                __ErrorMsg = ex.Message;
                return false;
            }
        }

        public string GenerateJwtToken(string namaUser, IConfiguration pConfig)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(pConfig["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, namaUser),
                new Claim(ClaimTypes.Role, "1"),
            };
            var token = new JwtSecurityToken(pConfig["Jwt:Issuer"],
                pConfig["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }



    }


}